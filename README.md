# EcommercePiorApi

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-Web_API-512BD4?style=for-the-badge&logo=dotnet)
![Entity Framework](https://img.shields.io/badge/Entity_Framework_Core-512BD4?style=for-the-badge)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![Git](https://img.shields.io/badge/Git-F05032?style=for-the-badge&logo=git&logoColor=white)
![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github)

## Sobre o projeto

O EcommercePiorApi é uma API ASP.NET Core desenvolvida como parte de um desafio de Qualidade de Código, Arquitetura e Refatoração.

O projeto foi disponibilizado inicialmente com diversos problemas intencionais de arquitetura, organização e manutenção. A proposta do desafio foi evoluir a aplicação aplicando conceitos como SOLID, Injeção de Dependência, separação de responsabilidades, camadas de serviço e acesso a dados.

---

## Conceitos aplicados

![SOLID](https://img.shields.io/badge/SOLID-Principles-blue?style=for-the-badge)
![Dependency Injection](https://img.shields.io/badge/Dependency_Injection-DI-green?style=for-the-badge)
![Repository Pattern](https://img.shields.io/badge/Repository-Pattern-orange?style=for-the-badge)
![Service Layer](https://img.shields.io/badge/Service-Layer-red?style=for-the-badge)
![Clean Code](https://img.shields.io/badge/Clean-Code-2ea44f?style=for-the-badge)

---

## Tecnologias utilizadas

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger / OpenAPI

---

## Arquitetura do projeto

A aplicação foi reorganizada seguindo uma estrutura em camadas para melhorar a manutenção, legibilidade e escalabilidade do código.

```text
EcommercePiorApi/
│
├── Controllers/
│   ├── EstoqueController.cs
│   ├── PedidosController.cs
│   └── ProdutosController.cs
│
├── Data/
│   └── AppDbContext.cs
│
├── DTOs/
│   ├── AtualizarProdutoDTO.cs
│   ├── CriarProdutoDTO.cs
│   ├── ItemPedidoResponseDTO.cs
│   ├── ItemRequest.cs
│   ├── PedidoRequest.cs
│   ├── PedidoResponseDTO.cs
│   ├── ProdutoResponseDTO.cs
│   └── ReabastecerEstoqueDTO.cs
│
├── Enums/
│   └── StatusPedido.cs
│
├── Exceptions/
│
├── Middleware/
│
├── Models/
│   ├── Produto.cs
│   ├── Pedido.cs
│   └── ItemPedido.cs
│
├── Repositories/
│   ├── Interfaces/
│   ├── PedidoRepository.cs
│   └── ProdutoRepository.cs
│
├── Services/
│   ├── Interfaces/
│   ├── DescontoService.cs
│   ├── EstoqueService.cs
│   ├── PedidoService.cs
│   └── ProdutoService.cs
│
├── appsettings.json
├── EcommercePiorApi.csproj
├── LogisticaMock.cs
├── PagamentoMock.cs
├── Program.cs
├── README.md
└── Utils.cs
```

---

## Responsabilidade das camadas

### Controllers

Responsáveis por receber as requisições HTTP e retornar as respostas da API.

### DTOs

Responsáveis pela transferência de dados entre cliente e aplicação, evitando expor diretamente as entidades do domínio.

### Models

Representam as entidades de negócio da aplicação.

Exemplos:

- Produto
- Pedido
- ItemPedido

### Services

Contêm as regras de negócio da aplicação.

Exemplos:

- Processamento de pedidos
- Controle de estoque
- Aplicação de descontos

### Repositories

Responsáveis pelo acesso aos dados utilizando o Entity Framework.

### Data

Contém a configuração do banco de dados através do `AppDbContext`.

### Middleware

Destinado ao tratamento global das requisições e exceções da aplicação.

### Exceptions

Responsável pelas exceções customizadas do domínio.

---

## Melhorias implementadas

### Estrutura e organização

- Separação de responsabilidades por camadas.
- Criação de Controllers específicos por domínio.
- Criação das camadas Repository e Service.
- Organização dos DTOs.
- Separação dos Models em arquivos individuais.

### Banco de dados

- Remoção da connection string hardcoded.
- Utilização de `appsettings.json`.
- Configuração do Entity Framework através de Dependency Injection.
- Centralização da configuração do banco em `AppDbContext`.

### Qualidade de código

- Redução do acoplamento.
- Aplicação de princípios SOLID.
- Organização das responsabilidades por domínio.
- Maior facilidade para manutenção e evolução do projeto.

---

## Funcionalidades

### Produtos

- Listar produtos
- Buscar produto por ID
- Cadastrar produto
- Atualizar produto
- Remover produto

### Estoque

- Consultar estoque
- Reabastecer estoque

### Pedidos

- Criar pedido
- Consultar pedidos
- Buscar pedido por ID

### Pagamento

- Simulação de processamento de pagamento

### Logística

- Simulação de geração de código de rastreio

---

## Como executar o projeto

### 1. Clonar o repositório

```bash
git clone <url-do-repositorio>
```

### 2. Entrar na pasta do projeto

```bash
cd EcommercePiorApi
```

### 3. Restaurar dependências

```bash
dotnet restore
```

### 4. Compilar o projeto

```bash
dotnet build
```

### 5. Executar a aplicação

```bash
dotnet run
```

O banco SQLite será criado automaticamente na primeira execução.

---

## Documentação da API

Após iniciar a aplicação, acesse o Swagger pela URL exibida no terminal.

Exemplo:

```text
https://localhost:xxxx/swagger
```

ou

```text
http://localhost:xxxx/swagger
```

---

## Equipe

| Participante | Responsabilidade |
|-------------|------------------|
| Renata Aires | Infraestrutura, Models e Data |
| Maria Vitória Espínola Carneiro | Repositories e contratos de persistência |
| Ana Carla Gomes da S. Lima | Services de produtos, estoque e pedidos |
| Samla Manathe de Souza Domingos | Controllers e DTOs |
| Thaislaine Erica T. dos Reis | Pagamentos, logística, tratamento de erros e testes |

---

## Objetivos alcançados

- Aplicação dos princípios SOLID.
- Uso de Injeção de Dependência.
- Organização em camadas.
- Separação das responsabilidades.
- Refatoração de código legado.
- Estrutura preparada para manutenção e evolução futura.
- Trabalho colaborativo utilizando Git e GitHub.