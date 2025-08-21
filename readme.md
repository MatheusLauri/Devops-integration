# API de Integração para Azure DevOps 🚀

Um serviço .NET 8 construído para receber, processar e registrar eventos de webhooks do Azure DevOps, focado em capturar o momento exato em que um item de trabalho entra na fase de "In Progress" pela primeira vez.

---

## 📜 Descrição

Este projeto resolve um problema comum no acompanhamento de projetos: saber quando o trabalho em uma tarefa, bug ou user story realmente começou. Em vez de registrar cada pequena alteração, esta API atua como um "porteiro" inteligente que:

- Ouve os eventos de atualização de itens de trabalho do Azure DevOps.
- Filtra e reage apenas quando um item é movido para o estado "In Progress".
- Verifica se este é o primeiro registro daquele item, evitando duplicatas.
- Salva informações ricas sobre o item (ID, Título, Descrição e o Evento) em um banco de dados SQL Server para futuras análises ou integrações.

---

## ✨ Funcionalidades Principais

- **Gatilho Inteligente:** A API só age em eventos relevantes (`workitem.updated` para o estado "In Progress").
- **Prevenção de Duplicidade:** Garante que cada item de trabalho seja registrado apenas uma vez, na primeira vez que entra em progresso.
- **Extração de Dados Ricos:** Captura não apenas o ID, mas também o Título e a Descrição completos do item de trabalho.
- **Configuração Segura:** Utiliza um arquivo `.env` para gerenciar a string de conexão do banco de dados, mantendo-a fora do controle de versão.
- **Estrutura Moderna:** Construído com a arquitetura Minimal API do .NET 8 e Entity Framework Core para acesso a dados.

---

## 🛠️ Tecnologias Utilizadas

- ASP.NET Core 8
- Entity Framework Core 8
- SQL Server
- Ngrok (para desenvolvimento local)
- Azure DevOps (como fonte dos webhooks)

---

## 🚀 Guia de Instalação e Configuração

Siga os passos abaixo para ter o projeto rodando em seu ambiente local.

### 1. Pré-requisitos

- .NET 8 SDK
- SQL Server (qualquer edição)
- Git
- Ngrok

### 2. Clone o Repositório

```bash
git clone https://github.com/MatheusLauri/Devops-integration.git
cd IntegracaoDevOps
```

### 3. Configure o Banco de Dados

A aplicação precisa da tabela `TS_UPGDEVOPS` para funcionar. Execute o script SQL abaixo no seu banco de dados:

```sql
CREATE TABLE [dbo].[TS_UPGDEVOPS](
    [PK_ID] [int] IDENTITY(1,1) NOT NULL,
    [DS_US] [nvarchar](max) NULL,
    [DS_EVENTO] [nvarchar](max) NULL,
    [DS_TITULO] [nvarchar](max) NULL,
    [DS_DESCRIÇÃO] [nvarchar](max) NULL,
 CONSTRAINT [PK_TS_UPGDEVOPS] PRIMARY KEY CLUSTERED ([PK_ID] ASC)
);
```

### 4. Configure as Variáveis de Ambiente

Na raiz do projeto, crie um arquivo chamado `.env` e adicione sua string de conexão:

```ini
ConnectionStrings__DefaultConnection="Server=SEU_SERVIDOR;Database=SEU_BANCO;User Id=SEU_USUARIO;Password=SUA_SENHA;TrustServerCertificate=True"
```

### 5. Execute as Migrations do EF Core

Este passo garante que o modelo de código está sincronizado com o banco (mesmo que já tenhamos criado a tabela manualmente).

```bash
dotnet ef database update
```

### 6. Execute a Aplicação

```bash
dotnet run
```

Anote a porta em que a aplicação está rodando (ex: http://localhost:5226).

### 7. Configure o Túnel com Ngrok e o Webhook

Em um novo terminal, inicie o Ngrok:

```bash
ngrok http http://localhost:5226
```
(Troque 5226 pela porta correta)

Copie o endereço `https://...` gerado pelo Ngrok.

No Azure DevOps, vá para **Project Settings > Service Hooks** e crie uma nova inscrição ("+").

- **Serviço:** Web Hooks
- **Gatilho (Trigger):** Work item updated
- **Filtros:** Se desejar, filtre por Área, Tipo de Item, etc.
- **URL:** Cole o endereço do Ngrok seguido do endpoint:  
  `https://SEU_ENDERECO.ngrok-free.app/api/webhook/userstory-updated`

Clique em **Test** para verificar e em **Finish** para salvar.

---

## ⚙️ Como Funciona

O fluxo lógico da aplicação é o seguinte:

```
Azure DevOps (Item movido para "In Progress")
    ↓
Webhook
    ↓
Ngrok
    ↓
API .NET
    ↓
[Filtro 1: O estado é "In Progress"?]
    ↓
[Filtro 2: O ID já existe no banco?]
    ↓
INSERT no SQL Server
```

---

## 🎛️ Detalhes do Endpoint

- **Método:** POST
- **URL:** `/api/webhook/userstory-updated`
- **Corpo (Body):** Espera o payload JSON padrão do Azure DevOps para o evento `workitem.updated`.

**Respostas:**
- `200 OK`: A requisição foi recebida com sucesso (mesmo que nenhuma ação tenha sido tomada).
- `400 Bad Request`: O JSON recebido é inválido ou incompleto.
- `500 Internal Server Error`: Ocorreu um erro inesperado no servidor (ex: falha de conexão com o banco).

---

## 🔮 Próximos Passos e Melhorias

- [ ] Implementar uma camada de Serviço (Service Layer) para separar a lógica de negócio do Controller.
- [ ] Adicionar autenticação ao endpoint para maior segurança.
- [ ] Criar testes unitários e de integração.
- [ ] Expandir a lógica para lidar com outros estados (ex: "Done", "In QA").
- [ ] Configurar uma pipeline de CI/CD para fazer o deploy automático para um ambiente na nuvem (como o Azure App Service).