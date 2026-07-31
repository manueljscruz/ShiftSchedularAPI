# Subscription Schema

## Contexto geral

Este documento descreve o schema de base de dados para o módulo de subscrições, incluindo as decisões de negócio tomadas durante o design. O sistema suporta entidades com hierarquias (entidade pai / sub-entidades), billing centralizado ou independente por sub-entidade, pricing variável com base no número de membros únicos, e geração de horários como recurso consumível.

> **Nota de estado:** este documento assume o **cenário intermédio de pricing por duração** como caminho de trabalho — cada combinação tipo-de-plano+duração (`SubscriptionPlanDurationPrice`) tem o seu próprio preço, visibilidade e elegibilidade a campanhas, em vez de `SubscriptionPlanType` ter um preço único com desconto percentual por duração. Ainda **não é uma decisão final** — este é o desenho completo para permitir avaliar o cenário por inteiro antes de decidir se avança.

> **Ver também:** as obrigações legais de faturação (fatura obrigatória, IVA, elementos fiscais) estão documentadas separadamente em [invoicing-compliance.md](invoicing-compliance.md) — ainda sem desenho de schema associado.

---

## Decisões de negócio relevantes

### Pricing por duração
- Cada combinação tipo-de-plano+duração tem pricing totalmente independente: `SubscriptionPlanDurationPrice` guarda `BasePrice`, `ScaleRequirement`, `PricePerExtraMember`, `IncludedGenerations`, `PricePerExtraGeneration` por combinação — não é "preço mensal × N com desconto"
- `SubscriptionPlanType` fica reduzido a identidade do tier (`Name`, `Description`); todo o pricing e as flags comerciais vivem na combinação
- A fórmula de faturação por período é: `Total = BasePrice + (ExtraMembers × PricePerExtraMember) + (ExtraGenerations × PricePerExtraGeneration)`, usando os valores da combinação subscrita
- Todos os valores monetários são do tipo **`decimal`** — não `float`/`double` (evita problemas de precisão de ponto flutuante) e não inteiro em cêntimos (decisão revista; `decimal` já representa valores monetários com exatidão em C#/SQL Server, sem necessidade de trabalhar em cêntimos)
- `SubscriptionPlanDurationPrice` tem **PK própria** (`SubscriptionPlanDurationPriceId`) + `UNIQUE(SubscriptionPlanTypeId, SubscriptionDurationTypeId)`, em vez de chave composta — deixa margem a histórico de preços por combinação no futuro (ex: `EffectiveFrom`/`IsActive`) sem migrar a PK
- Um tier pode não estar disponível em todas as durações — basta não existir a linha (ex: "Free" só em Mensal)
- `PromoPercent` mantém-se em `SubscriptionDurationType`, mas passa a ser **informativo/de apresentação**, não parte da fórmula de faturação: o `BasePrice` de cada combinação já é o valor final. `PromoPercent` fica disponível para a UI mostrar algo como "poupa 20% face ao mensal", calculado a partir do `BasePrice` da combinação e do `PromoPercent` — sem influenciar o `TotalCharged` real

### Modelo conceptual: tier × duração
- `SubscriptionPlanType` já não representa "o plano" no sentido comercial — é só o eixo qualitativo do tier (ex: Free, Trial, Pro, Enterprise). Renomeado de `SubscriptionPlan` para refletir isto: com preço e visibilidade movidos para a combinação, o que sobra é um **tipo/categoria**, não uma oferta subscrivível por si só
- `SubscriptionDurationType` é o outro eixo — a cadência de faturação (Mensal, Anual, 14 dias de trial, etc.)
- `SubscriptionPlanDurationPrice` é a célula concreta desta matriz — a combinação real e vendável que o cliente efetivamente subscreve (o que um cliente chamaria de "o meu plano", ex: "estou no Pro Anual")
- **`Description` mantém-se em `SubscriptionPlanType`**, como descrição interna do tier (o que distingue "Enterprise" de "Pro" conceptualmente) — evita duplicar o mesmo texto por cada combinação em `SubscriptionPlanDurationPrice`, o mesmo argumento usado para não duplicar a localização (ver "Localização"). Se no futuro for preciso texto de marketing que varie por duração (ex: "poupa X% no compromisso anual"), isso é uma preocupação diferente de `Description` e só justificaria um campo/tabela nova quando houver essa necessidade concreta — não se constrói agora especulativamente
- **Tabelas adicionais:** não é preciso nenhuma para já — a estrutura atual (`SubscriptionPlanType` + `SubscriptionDurationType` + `SubscriptionPlanDurationPrice`) já cobre o modelo tier × duração × preço/visibilidade de forma limpa

### Visibilidade de planos
- `IsPublicPlan` controla se uma combinação tipo-de-plano+duração aparece na listagem pública para o cliente escolher; combinações com `IsPublicPlan = false` só podem ser atribuídas manualmente (ex: acordos particulares, planos enterprise à medida) — não aparecem no self-service
- Granularidade ao nível da combinação, não do tier inteiro — o mesmo tier pode ser público numa duração e reservado noutra (ex: "Pro" público em Mensal e Anual, mas a opção Trimestral desse mesmo tier só atribuída manualmente). Por isso `IsPublicPlan` vive em `SubscriptionPlanDurationPrice`, não em `SubscriptionPlanType`

### Membros únicos
- Uma entidade pode ter sub-entidades; um utilizador pode pertencer a várias sub-entidades
- A contagem de membros para billing é feita por **membros únicos** sob a `BillingEntity`, usando `COUNT(DISTINCT UserId)`
- Isto evita cobrar em duplicado por utilizadores partilhados entre sub-entidades

### Hierarquia de entidades e billing
- `Entity.ParentEntityId` define a hierarquia (nullable para entidades raiz)
- `Entity.BillingEntityId` define quem paga a conta — pode ser a própria entidade ou a entidade pai
- Cenário A (billing centralizado): sub-entidades apontam `BillingEntityId` para a entidade pai
- Cenário B (billing independente): cada sub-entidade aponta `BillingEntityId` para si própria
- O `CountryCode` relevante para métodos de pagamento é sempre o da `BillingEntity`

### Gerações de horários
- Gerações são assíncronas — devem ser processadas em background (Hangfire ou Worker Service, a decidir)
- Só gerações com `Status = Completed` contam para o limite de gerações usadas no período
- O plano free tem um limite baixo de gerações (ex: 3/mês); planos pagos têm limite alto (ex: 30/mês) que na prática nunca é atingido em uso normal — serve como proteção de infraestrutura, não como monetização

### Renovações
- Cada renovação cria um **novo registo** `EntitySubscriptionPlan` — o anterior fica intacto para histórico
- `PreviousSubscriptionPlanId` encadeia os registos de renovação
- `Status` indica o estado atual: `Active`, `Expired`, `Cancelled`
- Uma renovação pode mudar de combinação tipo-de-plano+duração (ex: de "Pro Mensal" para "Pro Anual") — basta o novo registo apontar para outro `SubscriptionPlanDurationPriceId`

### Pagamentos
- **Gateway escolhido: Stripe.** `PaymentMethod.Token`/`GatewayCustomerId`/`EntitySubscriptionPayment.GatewayTransactionId` referem-se a IDs do Stripe (ex: `pm_xxx`, `cus_xxx`, `pi_xxx`) — deixa de ser só um exemplo ilustrativo, é a decisão tomada
- O gateway de pagamento nunca devolve dados raw de cartão — apenas tokens
- Uma entidade pode ter múltiplos métodos de pagamento; `IsDefault` indica o usado por omissão nas renovações
- Um `SubscriptionBillingRecord` pode ter múltiplos `EntitySubscriptionPayment` — tentativas falhadas geram novos registos de pagamento sem criar novo registo de faturação
- `PaymentWebhookEvent` é independente (sem FK obrigatória) para garantir que eventos são sempre guardados antes de processados; `EntitySubscriptionPaymentId` é nullable e preenchido após processamento
- **Semântica de `PaymentMethodTypeCountry` decidida:** um `PaymentMethodType` **sem nenhuma linha** em `PaymentMethodTypeCountry` está disponível em **todos os países** (ex: Cartão, PayPal); um `PaymentMethodType` **com linhas** fica restrito **só** aos países listados (ex: MB WAY só `PT`). Ainda por implementar: a query/endpoint que, dado o país da `BillingEntity`, decide quais os métodos disponíveis — não existe ainda, só o CRUD de admin
- Prioridade de métodos a configurar: Cartão (Stripe nativo) → SEPA Direct Debit (Stripe nativo, recorrente B2B UE) → MB WAY (precisa de agregador PT tipo Ifthenpay/EuPago por cima do Stripe, já que o Stripe não o suporta nativamente) → PayPal (baixa prioridade, mais orientado a B2C pontual do que subscrições recorrentes)

### Snapshots de faturação
- `SubscriptionBillingRecord` guarda snapshots dos preços da combinação tipo-de-plano+duração no momento da cobrança (`BasePrice`, `PricePerExtraMember`, etc., vindos de `SubscriptionPlanDurationPrice`)
- Isto garante que o histórico de faturação não é afetado por alterações futuras aos preços das combinações

### Campanhas
- Uma campanha aplica-se a um subconjunto de combinações tipo-de-plano+duração, definido pela tabela `CampaignSchedulePlan`
- `RedemptionCount` é incrementado atomicamente na mesma transação que cria o `EntitySubscriptionPlan`
- O incremento deve ser feito com `UPDATE Campaign SET RedemptionCount = RedemptionCount + 1` dentro da transação
- Granularidade ao nível da combinação, não do tier inteiro — o alvo de uma campanha é uma decisão comercial sobre uma oferta específica (ex: "-20% só na opção Anual do Pro", para incentivar o compromisso longo). `CampaignSchedulePlan` referencia `SubscriptionPlanDurationPriceId` (FK simples). Para aplicar uma campanha ao tier inteiro independentemente da duração, é necessária uma linha por combinação desse tier

### Localização
- O padrão de localização é aplicado a `Campaign`, `SubscriptionPlanType`, `SubscriptionDurationType` e `PaymentMethodType`
- Cada entidade tem uma tabela `*Localization` com FK para a tabela `Localization` base
- `CountryCode` segue o standard ISO 3166-1 alpha-2 (ex: `PT`, `DE`) — campo `CHAR(2)` fixo
- `SubscriptionPlanDurationPrice` não tem localização própria — `Name`/`Description` continuam a viver (e a ser traduzidos) ao nível do tier em `SubscriptionPlanType`/`SubscriptionPlanTypeLocalization`

---

## Tabelas

### `SubscriptionPlanType`
Representa a identidade de um tier de subscrição disponível na plataforma (ex: "Free", "Trial", "Pro", "Enterprise"). Não guarda preço nem visibilidade — isso vive em `SubscriptionPlanDurationPrice`.

| Campo | Tipo | Descrição |
|---|---|---|
| `SubscriptionPlanTypeId` | PK | Chave primária |
| `Name` | string | Nome interno do tier (ex: "Basic", "Pro") |
| `Description` | string | Descrição interna do tier |

---

### `SubscriptionPlanTypeLocalization`
Traduções dos textos do tipo de plano.

| Campo | Tipo | Descrição |
|---|---|---|
| `SubscriptionPlanTypeId` | FK | Chave composta |
| `LocalizationId` | FK | Chave composta |
| `SubscriptionPlanTypeNameDisplayValue` | string | Nome localizado do tipo de plano |
| `SubscriptionPlanTypeDescriptionDisplayValue` | string | Descrição localizada do tipo de plano |

---

### `SubscriptionDurationType`
Define os tipos de duração disponíveis para subscrições.

| Campo | Tipo | Descrição |
|---|---|---|
| `DurationTypeId` | PK | Chave primária |
| `Name` | string | Nome interno (ex: "Mensal", "Anual") |
| `Days` | int | Número de dias que esta duração representa |
| `AppliesPromo` | bool | Flag que indica se esta duração é elegível para desconto de campanha |
| `PromoPercent` | decimal | Percentagem informativa/de apresentação (ex: "poupa 20%") — não entra na fórmula de faturação, o preço real vem de `SubscriptionPlanDurationPrice.BasePrice` |

---

### `SubscriptionDurationTypeLocalization`
Traduções dos textos do tipo de duração.

| Campo | Tipo | Descrição |
|---|---|---|
| `DurationTypeId` | FK | Chave composta |
| `LocalizationId` | FK | Chave composta |
| `SubscriptionDurationTypeDisplayValue` | string | Nome localizado do tipo de duração |

---

### `SubscriptionPlanDurationPrice`
Preço e disponibilidade de uma combinação específica tipo-de-plano+duração. É a unidade real de "oferta" que o cliente subscreve.

| Campo | Tipo | Descrição |
|---|---|---|
| `SubscriptionPlanDurationPriceId` | PK | Chave primária própria |
| `SubscriptionPlanTypeId` | FK | O tier (parte de `UNIQUE(SubscriptionPlanTypeId, SubscriptionDurationTypeId)`) |
| `SubscriptionDurationTypeId` | FK | A duração (parte de `UNIQUE(SubscriptionPlanTypeId, SubscriptionDurationTypeId)`) |
| `BasePrice` | decimal | Preço final desta combinação, até ao limite de membros incluídos |
| `ToScale` | bool | Flag que indica se esta combinação aplica pricing variável acima do threshold |
| `ScaleRequirement` | int | Número máximo de membros incluídos no `BasePrice` |
| `PricePerExtraMember` | decimal | Acréscimo por cada membro acima do `ScaleRequirement` |
| `IncludedGenerations` | int | Número de gerações incluídas por período de faturação |
| `PricePerExtraGeneration` | decimal nullable | Acréscimo por cada geração acima do limite |
| `IsPublicPlan` | bool | Se `false`, esta combinação não é mostrada aos clientes na listagem pública — reservada para atribuição manual |
| `IsActive` | bool | Permite desativar temporariamente esta combinação sem apagar a linha |

---

### `Campaign`
Representa uma campanha promocional aplicável a combinações tipo-de-plano+duração.

| Campo | Tipo | Descrição |
|---|---|---|
| `CampaignId` | PK | Chave primária |
| `Name` | string | Nome interno da campanha |
| `Description` | string | Descrição interna da campanha |
| `StartDate` | date | Data de início |
| `EndDate` | date | Data de fim |
| `PromotionPercent` | decimal | Percentagem de desconto aplicada |
| `IsActive` | bool | Flag para pausar/desativar a campanha manualmente |
| `MaxRedemptions` | int | Limite de adesões (nullable = sem limite) |
| `RedemptionCount` | int | Contador de adesões — incrementado atomicamente na transação de subscrição |
| `CouponCode` | string | Código necessário para ativar a campanha (nullable = aplicação automática) |

---

### `CampaignLocalization`
Traduções dos textos da campanha.

| Campo | Tipo | Descrição |
|---|---|---|
| `CampaignId` | FK | Chave composta |
| `LocalizationId` | FK | Chave composta |
| `CampaignNameDisplayValue` | string | Nome localizado da campanha |
| `CampaignDescriptionDisplayValue` | string | Descrição localizada da campanha |

---

### `CampaignSchedulePlan`
Tabela de junção — define quais as combinações tipo-de-plano+duração disponíveis dentro de uma campanha.

| Campo | Tipo | Descrição |
|---|---|---|
| `CampaignId` | FK | Chave composta |
| `SubscriptionPlanDurationPriceId` | FK | Chave composta — a combinação específica visada pela campanha |

---

### `EntitySubscriptionPlan`
Instância concreta de uma subscrição — representa o contrato ativo entre uma entidade e uma combinação tipo-de-plano+duração. Cada renovação cria um novo registo.

| Campo | Tipo | Descrição |
|---|---|---|
| `EntitySubscriptionPlanId` | PK | Chave primária |
| `EntityId` | FK | A entidade que subscreveu |
| `SubscriptionPlanDurationPriceId` | FK | A combinação tipo-de-plano+duração escolhida |
| `CampaignId` | FK nullable | A campanha aplicada no momento da subscrição |
| `PreviousSubscriptionPlanId` | FK nullable | Aponta para o registo do período anterior — null no primeiro registo |
| `StartDate` | date | Data de início deste período |
| `EndDate` | date | Data de fim — calculada a partir de StartDate + Days da duração escolhida |
| `Status` | string | Estado: `Active` / `Expired` / `Cancelled` |

---

### `SubscriptionBillingRecord`
Registo de faturação por período. Guarda snapshots dos preços da combinação no momento da cobrança.

| Campo | Tipo | Descrição |
|---|---|---|
| `BillingRecordId` | PK | Chave primária |
| `EntitySubscriptionPlanId` | FK | O período de subscrição a que esta faturação diz respeito |
| `PeriodStart` | date | Data de início do período faturado |
| `PeriodEnd` | date | Data de fim do período faturado |
| `UniqueMemberCountSnapshot` | int | Número de membros únicos da BillingEntity no momento da faturação |
| `BasePrice` | decimal | Snapshot do `BasePrice` da combinação no momento da faturação |
| `ExtraMembers` | int | MAX(0, UniqueMemberCountSnapshot - ScaleRequirement) |
| `PricePerExtraMember` | decimal | Snapshot do preço por membro extra |
| `IncludedGenerations` | int | Snapshot do número de gerações incluídas |
| `GenerationsUsed` | int | Número de gerações com Status = Completed neste período |
| `ExtraGenerations` | int | MAX(0, GenerationsUsed - IncludedGenerations) |
| `PricePerExtraGeneration` | decimal | Snapshot do preço por geração extra |
| `TotalCharged` | decimal | Total cobrado |

---

### `EntitySubscriptionPayment`
Registo de cada tentativa de pagamento de um billing record. Um billing record pode ter múltiplos pagamentos (tentativas falhadas + tentativa bem sucedida).

| Campo | Tipo | Descrição |
|---|---|---|
| `EntitySubscriptionPaymentId` | PK | Chave primária |
| `EntitySubscriptionPlanId` | FK | A subscrição a que este pagamento diz respeito |
| `BillingRecordId` | FK | O registo de faturação que originou este pagamento |
| `PaymentMethodId` | FK | O método de pagamento usado nesta tentativa |
| `GatewayTransactionId` | string | ID da transação no gateway (ex: `pi_xxx`) |
| `Status` | string | Estado: `Pending` / `Succeeded` / `Failed` / `Refunded` |
| `CreatedAt` | datetime | Data e hora em que a tentativa foi iniciada |
| `FailureCode` | string nullable | Código de erro do gateway em caso de falha |
| `FailureMessage` | string nullable | Mensagem de erro legível do gateway |

---

### `PaymentMethod`
Métodos de pagamento guardados por uma entidade. Nunca contém dados raw de cartão — apenas tokens do gateway.

| Campo | Tipo | Descrição |
|---|---|---|
| `PaymentMethodId` | PK | Chave primária |
| `EntityId` | FK | A BillingEntity a quem pertence este método |
| `GatewayCustomerId` | string | ID do cliente no gateway (ex: `cus_xxx`) |
| `PaymentMethodTypeId` | FK | Tipo de método de pagamento |
| `Last4` | string nullable | Últimos 4 dígitos do cartão |
| `CardBrand` | string nullable | Marca do cartão (ex: `visa`, `mastercard`) |
| `ExpiryMonth` | int nullable | Mês de expiração |
| `ExpiryYear` | int nullable | Ano de expiração |
| `Token` | string | Token do gateway — nunca dados raw |
| `IsDefault` | bool | Flag que indica o método usado por omissão nas renovações |
| `IsActive` | bool | Flag que indica se o método está ativo |

---

### `PaymentMethodType`
Tipos de método de pagamento disponíveis na plataforma.

| Campo | Tipo | Descrição |
|---|---|---|
| `PaymentMethodTypeId` | PK | Chave primária |
| `Code` | string | Código interno (ex: `card`, `mbway`, `sepa_debit`) |
| `IsActive` | bool | Flag que indica se este tipo está disponível na plataforma |

---

### `PaymentMethodTypeCountry`
Define em que países cada tipo de método de pagamento está disponível. Usado para filtrar métodos disponíveis com base no CountryCode da BillingEntity.

| Campo | Tipo | Descrição |
|---|---|---|
| `PaymentMethodTypeId` | FK | Chave composta |
| `CountryCode` | CHAR(2) | Código ISO 3166-1 alpha-2 (ex: `PT`, `DE`) — chave composta |

---

### `PaymentMethodTypeLocalization`
Traduções dos nomes dos tipos de método de pagamento.

| Campo | Tipo | Descrição |
|---|---|---|
| `PaymentMethodTypeId` | FK | Chave composta |
| `LocalizationId` | FK | Chave composta |
| `PaymentMethodTypeDisplayValue` | string | Nome localizado do tipo de pagamento |

---

### `ScheduleGeneration`
Registo de cada geração de horários. Só gerações com `Status = Completed` contam para o limite do período de faturação.

| Campo | Tipo | Descrição |
|---|---|---|
| `ScheduleGenerationId` | PK | Chave primária |
| `EntitySubscriptionPlanId` | FK | A subscrição no âmbito da qual esta geração foi pedida |
| `GeneratedAt` | datetime | Data e hora em que a geração foi iniciada |
| `CompletedAt` | datetime nullable | Data e hora em que terminou — null enquanto em curso |
| `MemberCount` | int | Número de membros no momento da geração |
| `Status` | string | Estado: `Queued` / `Running` / `Completed` / `Failed` / `Cancelled` |
| `DurationMs` | int nullable | Duração total em milissegundos — calculado a partir de CompletedAt - GeneratedAt |

---

### `PaymentWebhookEvent`
Registo de eventos recebidos do gateway de pagamento. Intencionalmente sem FK obrigatória — eventos são guardados antes de processados para garantir que nunca se perde um evento.

| Campo | Tipo | Descrição |
|---|---|---|
| `WebhookEventId` | PK | Chave primária |
| `GatewayEventId` | string | ID único do evento no gateway — índice único para idempotência |
| `EventType` | string | Tipo de evento (ex: `payment_intent.succeeded`, `charge.refunded`) |
| `ReceivedAt` | datetime | Data e hora em que o evento foi recebido |
| `ProcessedAt` | datetime nullable | Data e hora em que foi processado — null até ao processamento |
| `EntitySubscriptionPaymentId` | FK nullable | Preenchido após processamento bem sucedido |
| `RawPayload` | string | JSON completo do evento — permite replay em caso de falha |

---

## Queries de referência

### Subscrição ativa de uma entidade
```sql
SELECT * FROM EntitySubscriptionPlan
WHERE EntityId = @entityId AND Status = 'Active'
```

### Combinações públicas disponíveis para um tipo de plano
```sql
SELECT spdp.*, sdt.Name AS DurationName
FROM SubscriptionPlanDurationPrice spdp
INNER JOIN SubscriptionDurationType sdt ON sdt.DurationTypeId = spdp.SubscriptionDurationTypeId
WHERE spdp.SubscriptionPlanTypeId = @subscriptionPlanTypeId
AND spdp.IsPublicPlan = 1
AND spdp.IsActive = 1
```

### Membros únicos de uma BillingEntity
```sql
SELECT COUNT(DISTINCT em.UserId)
FROM EntityMember em
INNER JOIN Entity e ON e.EntityId = em.EntityId
WHERE e.BillingEntityId = @billingEntityId
AND em.IsActive = 1
```

### Gerações usadas no período atual
```sql
SELECT COUNT(*) FROM ScheduleGeneration
WHERE EntitySubscriptionPlanId = @entitySubscriptionPlanId
AND GeneratedAt >= @periodStart
AND Status = 'Completed'
```

### Verificar idempotência de webhook
```sql
SELECT COUNT(*) FROM PaymentWebhookEvent
WHERE GatewayEventId = @gatewayEventId
```

### Fórmula de faturação
```sql
TotalCharged = BasePrice
             + (ExtraMembers * PricePerExtraMember)
             + (ExtraGenerations * PricePerExtraGeneration)
-- onde BasePrice, ScaleRequirement, PricePerExtraMember, IncludedGenerations e PricePerExtraGeneration
-- vêm da SubscriptionPlanDurationPrice da combinação subscrita (snapshot em SubscriptionBillingRecord)
-- ExtraMembers = MAX(0, UniqueMemberCountSnapshot - ScaleRequirement)
-- ExtraGenerations = MAX(0, GenerationsUsed - IncludedGenerations)
```
