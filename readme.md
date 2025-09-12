# API de Integração para Azure DevOps 🚀

Um serviço .NET 8 construído para receber, processar e registrar eventos de webhooks do Azure DevOps, focado em capturar os dados completos no momento exato em que uma User Story é criada.

---

## 📜 Descrição

Este projeto resolve um problema comum no acompanhamento de projetos: a necessidade de ter um registro detalhado e centralizado de novos itens de trabalho. Em vez de registrar cada pequena alteração, esta API atua como um "coletor" inteligente que:

- Ouve os eventos de criação de itens de trabalho (`workitem.created`) do Azure DevOps.
- Filtra e reage apenas quando o item criado é do tipo **User Story**.
- Verifica se este é o primeiro registro daquele item, evitando duplicatas.
- Salva uma gama rica de informações (ID, Título, Descrição, Solicitante, Responsável, Projeto, etc.) em um banco de dados SQL Server para futuras análises ou integrações.

---

## ✨ Funcionalidades Principais

- **Gatilho Específico:** A API só age no evento `workitem.created` e apenas para o tipo User Story.
- **Prevenção de Duplicidade:** Garante que cada User Story seja registrada apenas uma vez.
- **Extração de Dados Ricos:** Captura informações detalhadas, incluindo Título, Descrição, Solicitante (Nome e Email), Responsável (Nome e Email), Projeto, Área e Iteração.
- **Tratamento de Dados Flexível:** Lida de forma robusta com as inconsistências do Azure DevOps no formato dos dados de usuário (aceita tanto texto simples quanto objetos complexos).
- **Configuração Segura:** Utiliza um arquivo `.env` para gerenciar a string de conexão do banco de dados, mantendo-a fora do controle de versão.
- **Estrutura Sólida:** Construído com uma arquitetura Controller/Service em .NET 8 e usando Entity Framework Core para acesso a dados.

---

## 🛠️ Tecnologias Utilizadas

- ASP.NET Core 8
- Entity Framework Core 8
- SQL Server
- DotNetEnv (para carregar o arquivo .env)
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

A aplicação precisa da tabela `ts_upgdevops` para funcionar. Execute o script SQL abaixo no seu banco de dados:

```sql
CREATE TABLE ts_upgdevops
(
    PK_ID int identity PRIMARY KEY,
    FK_ITEM_TRABALHO_AZURE int,
    DS_TITULO nvarchar(255),
    DS_TIPO varchar(100),
    DS_DESCRICAO ntext,
    DS_ESTADO varchar(100),
    DS_MOTIVO varchar(255),
    DS_TAGS varchar(500),
    DS_SOLICITANTE_NOME nvarchar(255),
    DS_SOLICITANTE_EMAIL varchar(255),
    FK_SOLICITANTE_ID_AZURE varchar(100),
    DS_PROJETO_NOME varchar(255),
    DS_CAMINHO_AREA varchar(500),
    DS_CAMINHO_ITERACAO varchar(500),
    DS_RESPONSAVEL_NOME nvarchar(255),
    DS_RESPONSAVEL_EMAIL varchar(255),
    NR_PRIORIDADE int,
    DS_URL_UI varchar(1024),
    DS_URL_API varchar(1024),
    TG_INATIVO tinyint,
    FK_OWNER int,
    DH_INCLUSAO datetime2,
    DH_ALTERACAO datetime
);
```

### 4. Configure as Variáveis de Ambiente

Na raiz do projeto, crie um arquivo chamado `.env` e adicione sua string de conexão. O nome `ConnectionStrings__DefaultConnection` é importante para que o .NET a reconheça.

```ini
ConnectionStrings__DefaultConnection="Server=SEU_SERVIDOR;Database=SEU_BANCO;User Id=SEU_USUARIO;Password=SUA_SENHA;TrustServerCertificate=True"
```

### 5. Instale as Dependências

```bash
dotnet restore
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
- **Gatilho (Trigger):** Work item created
- **Filtros:**  
  Work Item Type = User Story
- **URL:** Cole o endereço do Ngrok seguido do endpoint correto:  
  `https://SEU_ENDERECO.ngrok-free.app/api/webhook/userstory-created`

Clique em **Test** para verificar e em **Finish** para salvar.

---

## ⚙️ Como Funciona

O fluxo lógico da aplicação é o seguinte:

```
Azure DevOps (Criação de uma nova User Story)
    ↓
Webhook
    ↓
Ngrok
    ↓
API .NET
    ↓
[Filtro 1: O evento é "workitem.created"?]
    ↓
[Filtro 2: O tipo é "User Story"?]
    ↓
[Filtro 3: O ID já existe no banco?]
    ↓
INSERT no SQL Server
```

---

## 🎛️ Detalhes do Endpoint

- **Método:** POST
- **URL:** `/api/webhook/userstory-created`
- **Corpo (Body):** Espera o payload JSON padrão do Azure DevOps para o evento `workitem.created`.

**Respostas:**
- `200 OK`: A requisição foi recebida com sucesso e processada (ou ignorada conforme as regras).
- `400 Bad Request`: O JSON recebido é inválido ou incompleto.
- `500 Internal Server Error`: Ocorreu um erro inesperado no servidor (ex: falha de conexão com o banco).

---

## 🔮 Próximos Passos e Melhorias

- [ ] Adicionar autenticação ao endpoint para maior segurança (ex: usando uma API Key).
- [ ] Criar testes unitários e de integração.
- [ ] Expandir a lógica para lidar com a atualização (`workitem.updated`) de User Stories.
- [ ] Configurar uma pipeline de CI/CD para fazer o deploy automático para um ambiente na nuvem 