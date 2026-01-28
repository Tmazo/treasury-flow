# Treasury Flow

Projeto exemplo para gerenciamento de transações e saldos de usuários.

## Visão geral

Arquitetura baseada em microserviços/split de responsabilidades com os seguintes componentes principais:

- `src/TreasuryFlow.Api` - API HTTP pública.
- `src/TreasuryFlow.AppHost` - Host de orquestração com `aspire` para execução local distribuída (SQL Server + RabbitMQ + serviços).
- `src/Workers/TreasuryFlow.Consumer` - Worker (consumer) que processa eventos de transação (MassTransit/RabbitMQ).
- `src/TreasuryFlow.Infrastructure` - Implementações de infra como EF Core, MassTransit, cache e comunicações.
- `src/TreasuryFlow.Application` - Regras de negócio e serviços.
- `src/TreasuryFlow.Domain` - Entidades de domínio.

## Execução local (recomenda-se Aspire CLI)

Este repositório foi pensado para execução local distribuída usando o Aspire (https://aspire.dev/pt-br/get-started/install-cli/).

1. Instale a CLI do Aspire seguindo: https://aspire.dev/pt-br/get-started/install-cli/
2. Faça clone do repositório:

   ```bash
   git clone https://github.com/Tmazo/treasury-flow.git
   cd treasury-flow
   ```

3. Execute a aplicação distribuída via `aspire` (exemplo):

   ```bash
   cd src/TreasuryFlow.AppHost
   aspire run
   ```

   Esse comando irá orquestrar os recursos (SQL Server, RabbitMQ) e iniciar os projetos configurados no `AppHost`.

Observação: Você também pode executar projetos individualmente via `dotnet run` dentro das pastas de cada projeto para depuração local.

## Padrões e bibliotecas usadas

- Clean Code / separação de camadas (Domain / Application / Infrastructure / Api).
- Validação com `FluentValidation` (ex.: validators em `src/TreasuryFlow.Api`).
- Mensageria com `MassTransit` + RabbitMQ (publisher/consumer).
- Persistência com Entity Framework Core (SQL Server).
- Logging e telemetria com OpenTelemetry (via `ServiceDefaults`).
- Aspire `ServiceDefaults` e `AppHost` para orquestração local de recursos.
- Background services para processamento assíncrono (Worker Service project).

## Endpoints principais

Veja os controladores em `src/TreasuryFlow.Api` (ex.: `TransactionsController`, `UserBalancesController`, `AuthController`).

## Observações de projeto e disclaimers

- Mantive a agregação no banco usando `GroupBy` com projeção direta para DTOs. A query é stateless, indexável e suficiente para o volume esperado. Caso o volume cresça, a evolução natural seria introduzir um read model específico.
- Este repositório é um ponto de partida; algumas decisões foram tomadas visando simplicidade e clareza para evolução futura.

## Melhorias / Roadmap sugerido

Itens já mapeados e outras sugestões para evolução:

- Adicionar Keycloak (centralizar autenticação/identidade).
- Implementar o padrão Outbox para garantir entrega de eventos de forma transacional.
- Implementar testes E2E integrados usando Aspire.
- Separar o Auth para um microserviço dedicado com banco de dados próprio.
- Implementar cache com Redis para consultas de leitura pesadas.
- Adicionar testes unitários e de integração automáticos (CI).
- Introduzir um read-model (CQRS) caso o volume de consultas por dia cresça muito.
- Melhorar observabilidade: dashboards, alertas e tracing configurado para ambientes.
- Harden security: validação de input mais rigorosa, proteção contra rate-limiting e configuração de CORS rígida.
- Adicionar pipelines CI/CD com GitHub Actions e verificação de análise estática (ex.: SonarQube).

## Diagrama da arquitetura

```mermaid
flowchart LR
  subgraph Infra
    DB[(SQL Server)]
    RMQ[(RabbitMQ)]
  end

  API["TreasuryFlow.Api\n(ASP.NET Core)"]
  Consumer["TreasuryFlow.Consumer\n(Worker MassTransit)"]
  App["TreasuryFlow.AppHost\n(Aspire)"]

  API --> DB
  API --> RMQ
  API -->|HTTP| App
  RMQ --> Consumer
  Consumer --> DB
  App --> DB
  App --> RMQ

  classDef infra fill:#f9f,stroke:#333,stroke-width:1px;
  class Infra infra;
```

> Disclaimer: Este README é um resumo gerado automaticamente e pode precisar de ajustes conforme o ambiente e as credenciais locais. Verifique as configurações de connection strings e secrets antes de executar em sua máquina.

## Busca do balance do dia

- A busca do balance do dia foi mantida com agregação por `GroupBy` e projeção direta para DTOs. Essa abordagem produz queries indexáveis e sem estado.


---

Se quiser, eu posso:

- Adicionar um `docker-compose` pronto para rodar SQL + RabbitMQ + app.
- Criar um `aspire` script de exemplo com parâmetros específicos.
- Traduzir o README para inglês.

Feito: README criado com documentação inicial e roadmap.
