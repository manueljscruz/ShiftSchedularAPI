# Compliance de Faturação

## Contexto e assunções

Este documento reúne o levantamento de obrigações legais relativas à emissão de faturas para o módulo de subscrições (ver [subscription-schema.md](subscription-schema.md)). É um documento separado porque a matéria é diferente — compliance legal/fiscal, não desenho de schema de subscrições.

Cenário assumido, confirmado durante a discussão:
- Empresa emissora sediada em **Portugal**
- Clientela pode ser de **Portugal, resto da UE, ou fora da UE**
- Subscritores podem ser **organizações (B2B) ou pessoas singulares (B2C)** — não é só B2B
- Abordagem preferida: **delegar a emissão do documento fiscal certificado** a um fornecedor terceiro (ex: Cegid Primavera, Moloni, InvoiceXpress) via API, em vez de certificar software próprio junto da AT — para minimizar trabalho de compliance interno

Este documento é um levantamento informativo, não aconselhamento jurídico/fiscal — os pontos mais específicos por país devem ser confirmados com um contabilista certificado antes de implementação.

---

## Obrigações legais em Portugal

Como a empresa emissora está sediada em Portugal, o **documento fiscal em si** segue sempre a lei portuguesa, independentemente de onde está o cliente.

- **Fatura obrigatória** — por lei (Código do IVA, art. 29º), a emissão de fatura é obrigatória sempre que há uma transmissão de bens ou prestação de serviços, incluindo pagamentos antecipados/por conta. Não é opcional
- **Elementos obrigatórios**:
  - Nome/denominação social, morada e NIF do fornecedor
  - Nome, morada e NIF do cliente quando sujeito passivo; numa fatura simplificada a consumidor final não é obrigatório nome/morada, mas o NIF é obrigatório sempre que o cliente o peça, seja qual for o valor
  - Descrição do serviço, valores, taxa de IVA aplicada
  - Número sequencial único por série
  - **ATCUD** (Código Único do Documento), visível junto ao número sequencial
  - **QR Code**, obrigatório desde 1 de janeiro de 2022 em faturas emitidas por software certificado
- **Software certificado pela AT** — obrigatório se faturação > 50.000€/ano no ano anterior, OU uso de software de faturação independentemente do volume, OU contabilidade organizada
- **SAF-T (PT)** — ficheiro standard de auditoria fiscal, comunicado mensalmente ao Portal das Finanças
- **Conservação** — 10 anos (Decreto-Lei 28/2019), com garantias de integridade (documento não pode ser alterável após emissão) e legibilidade

---

## Regras cross-border na UE

### B2B (cliente é uma organização)
- Mecanismo de **reverse charge** — não se cobra IVA português
- A fatura tem de incluir o número de IVA do cliente e uma menção do tipo "IVA — autoliquidação" / "reverse charge applies"
- O número de IVA do cliente deve ser **validado via VIES** antes de aplicar reverse charge — sem essa validação, o seguro é cobrar IVA português
- Continua a ser obrigatória a emissão de fatura, mesmo sem IVA cobrado

### B2C (cliente é uma pessoa singular)
- Limiar **EU-wide de 10.000€/ano** em vendas cross-border a consumidores da UE (fora de Portugal): abaixo disso, pode cobrar-se a taxa de IVA portuguesa a todos
- Ao ultrapassar o limiar, é necessário registo no regime **OSS** (One Stop Shop) e passar a cobrar a taxa de IVA do país de cada consumidor, reportado numa única declaração trimestral (sem necessidade de registo em cada país individualmente)
- Para determinar a que país um consumidor "pertence" para efeitos de IVA em serviços digitais, a UE exige **duas provas não contraditórias** da localização (ex: morada de faturação + país do cartão de pagamento, ou geolocalização de IP) — requisito de auditoria, não só formalidade

### Reforma ViDA
"VAT in the Digital Age" — faseamento até 2030, inclui e-invoicing obrigatório para B2B cross-border. Vale a pena acompanhar a médio prazo.

---

## Fora da UE

- **B2B** — geralmente fora do âmbito do IVA português: o lugar da prestação de serviços B2B é o país do cliente, pelo que o IVA português não é aplicável. Continua a ser obrigatória a emissão de fatura, com menção de operação não sujeita
- **B2C** — sem resposta genérica. Cada país tem as suas próprias regras para prestadores estrangeiros de serviços digitais (ex: UK tem VAT digital services tax própria, outros têm regimes semelhantes). A avaliar caso a caso com o contabilista à medida que o negócio expande para novos mercados

---

## Modelo de responsabilidades

Delegar a emissão do documento fiscal a um fornecedor certificado (ex: Primavera, Moloni, InvoiceXpress) não elimina a necessidade de o ShiftSchedular tratar de parte da lógica — só reduz o âmbito.

**O fornecedor certificado trata de:**
- Numeração sequencial e comunicação de séries à AT
- Geração do ATCUD e QR Code
- Submissão mensal do SAF-T
- Conformidade legal do formato do documento (certificação AT)

**O ShiftSchedular precisa de decidir e fornecer, antes de chamar a API do fornecedor:**
- Se o subscritor é organização ou pessoa singular (determina B2B vs B2C)
- O país "para efeitos de IVA" do cliente, com as duas provas não contraditórias guardadas (B2C UE)
- O tratamento de IVA aplicável: nacional / reverse charge (B2B UE, com NIF validado via VIES) / fora do âmbito (B2B fora da UE) / IVA do país do consumidor (B2C UE acima do limiar OSS)
- Nome legal, NIF/número de IVA e morada fiscal do cliente

A Cegid Primavera (marca que absorveu a Primavera BSS) tem um "Invoicing Engine" com API pensado para integração automática com e-commerce/negócios online, mas a documentação técnica pública é limitada — a integração real (endpoints, autenticação) só se confirma com acesso à documentação de parceiro deles.

---

## Nota de estado

O desenho de schema para suportar isto já foi feito, em duas entidades (só modelos C#, ainda sem `DbContext`):

- **`EntityBillingProfile`** — perfil fiscal "vivo" de uma entidade (nome legal, NIF/VAT, morada fiscal, classificação B2B/B2C, estado de validação VIES). Muda pouco, é gerido pelo cliente/admin.
- **`Invoice`** — o documento fiscal em si, ligado ao `SubscriptionBillingRecord` que originou a cobrança. Guarda um snapshot imutável dos dados fiscais do cliente e do tratamento de IVA aplicado no momento da emissão, mais os identificadores devolvidos pelo fornecedor externo certificado (`ExternalProviderInvoiceId`, `SeriesCode`, `DocumentNumber`, `ATCUD`, `DocumentUrl`) — estes ficam `null` até a integração com o fornecedor (Primavera/Moloni/etc.) existir de facto.

Ainda por fazer: configuração no `DbContext` (chaves, índices, incluindo o `UNIQUE` de validação de combinações), e a integração real com a API do fornecedor escolhido.
