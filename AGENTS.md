# AGENTS.md — mercado

Single-project WPF desktop app (`mercado.slnx`). .NET 10.0, `net10.0-windows`, WPF + MaterialDesignThemes 5.3.2. Este projeto visa auxiliar nos processos de vendas e gerenciamento administrativo de produtos.

## Build / run (verified from `mercado.csproj`)
- `dotnet build mercado.slnx` (or `dotnet run --project mercado/mercado.csproj`)
- No separate test project; no `dotnet test` target exists.

## Architecture at a glance
- `mercado/` = only project in solution.
- `ViewModel/` (`MainViewModel`, `PdvViewModel`, `EstoqueViewModel`, `PagamentoViewModel`) drives nav.
- `Service/` (`ProdutoService`, `VendaService`, `ReciboService`) does business logic.
- `Data/MercadoContext.cs` = EF Core `DbContext` with hardcoded connection: `Server=(localdb)\mssqllocaldb;Database=MercadoDB;Trusted_Connection=True;`. Requires SQL Server LocalDB installed.
- `Model/` (`Produto`, `Venda`, `ItemVenda`, `PagamentoVenda`) = EF entities.
- `View/` (.xaml + .cs) = WPF code-behind tied to ViewModels.

## High-signal gotchas
- `mercado/mercado.csproj` excludes `Model/Class1.cs` (`<Compile Remove="Model\Class1.cs" />`). Do not assume that file is used.
- `ReciboService` uses `System.Drawing.Printing.PrintDocument` for thermal receipt output. Printing depends on an actual printer being configured; `pd.PrinterSettings.PrinterName` is commented out (line 22).
- `VendaService.SalvarVendaCompleta` subtracts item quantities from `produtoNoBanco.QuantidadeAtual` inside a single `SaveChanges()` call. If `produtoNoBanco` is null, quantity is not adjusted but the sale is still saved.
- No migrations / `dotnet ef` workflow configured in repo. DB relies on `OnConfiguring` connection string; model changes require manual handling.
- Dependency: Material Design bundled theme (`MaterialDesign3.Defaults.xaml`) is loaded via `pack://application` URI in `App.xaml`. Removing the package breaks runtime.

## Users / roles (verified from app flow)
- **Administrador:** total access, but admin panel focuses exclusively on admin functions — stock control (`EstoqueVM`), reports, and user management.
- **Caixa:** access only to PDV (`PdvVM`) and sales management for their own user.

## Business rules (verified from code)
- Caixa panel only displays features the user has permission to use.
- Admin must be notified when products are near expiration (`Produto` has no explicit `Validade` field yet — verify model).
- Discounts (`Desconto`) and surcharges (`Acrescimo`) are applied at purchase time on `Venda`.
- Admin must be notified about low stock (check if implemented or still pending).
- Sales can be whole or fractional depending on `Produto.Fracionado` (see model property).
- Payment methods available: débito, crédito, pix, à vista (`PagamentoVenda` model).
- Cash register (caixa) is opened and closed daily — verify if this flow exists or is pending.
- Card machine fee (`taxa da maquininha`) must be calculated — verify if implemented or pending.
- Lost items management ("itens perdidos") — verify if implemented or pending.
- At least one administrator user must always exist in the system.

## What to skip
- No CI config, no pre-commit hooks, no lint/formatter scripts present.
- No tests, fixtures, or snapshot workflows.

## Commit Message Convention
- Use Conventional Commits format: `type(scope): description`
- Types: `feat`, `fix`, `refactor`, `docs`, `style`, `test`, `chore`, `perf`
- Scope: area affected (e.g., `PDV`, `Pagamento`, `Estoque`, `Model`, `VM`)
- Reference issues when applicable: `Closes #XX` or `Refs #XX`
- Examples:
  - `feat(PDV): Atalhos de teclado e melhorias no fluxo de venda`
  - `fix(Model): Correção no cálculo de subtotal com acréscimo`
  - `refactor(MainWindow): Feedback visual de seleção nos botões de navegação`

## Documentation (verified from repo structure)
- System requirements organized by module/epic → `docs/requisitos.md`
- Epic: `Venda` → requirements: VEN001 (listar), VEN002 (buscar), VEN003 (excluir), VEN004 (devolver), VEN005 (emitir nota não fiscal).
- Epic: `PDV` → requirements: PDV001 (processo de venda), PDV002 (beep do código do produto), PDV003 (incluir peso automático), PDV004 (emitir nota não fiscal).
- Epic: `Controle de Caixa` → requirements: CTC001 (listar lançamentos), CTC002 (novo lançamento avulso), CTC003 (alterar lançamento), CTC004 (apagar lançamento), CTC005 (apuração), CTC006 (sangria), CTC007 (exportar registro).
- **Padrão obrigatório para novos requisitos:** Módulo > Épico > REQ-ID (História de Usuário + Critérios [CA-N] + Regras de Negócio [BR-XX] + Prioridade [P0-P3] + Modelo de Dados relacionado).
