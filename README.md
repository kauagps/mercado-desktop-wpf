# Mercado

Sistema desktop em C# utilizando .NET com WPF. Este sistema tem finalidade de atender as demandas de um mercado, desde o processo de venda PDV, até apoio de decisões do gerente.

## Requisitos

- Windows com SQL Server LocalDB
- .NET 10 SDK

## Como executar

```powershell
dotnet build mercado.slnx
dotnet run --project mercado
```

## Banco de dados

A conexão está configurada em `mercado/Data/DbContext.cs` para o LocalDB:

```text
Server=(localdb)\mssqllocaldb;Database=MercadoDB;Trusted_Connection=True;
```

Para criar ou atualizar o schema, execute:

```powershell
dotnet ef database update --project mercado
```

## Primeiro acesso

Após aplicar as migrations, o banco contém o usuário administrador padrão:

- **Usuário:** `admin`
- **Senha:** `admin123`

Por segurança, o sistema **exige a troca dessa senha no primeiro login**. A nova senha deve ter pelo menos 8 caracteres, com pelo menos uma letra e um número.
