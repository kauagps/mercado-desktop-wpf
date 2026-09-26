# Requisitos do Sistema — Mercado

## Glossário

- **Caixa**: Sessão diária de vendas (abertura/fechamento obrigatórios).
- **TEF**: Transferência Eletrônica de Fundos (maquininha).
- **Nota não fiscal**: Comprovante simplificado sem validade fiscal.
- **Fracionado**: Produto vendido por peso ou fração (unidade < 1).

---

## Módulo: Venda

### Épico: Venda

**VEN001 - Listar registros de vendas**
- História de Usuário: Eu como gerente, quero que ao abrir o módulo de vendas, seja exibido uma tabela com todos os registros de vendas, pois assim de forma ágil já tenho um primeiro feedback visual de como está minhas vendas.
- Critérios de Aceite:
  - [CA-1] A listagem deve conter paginação inicial de 30, porém com opção de configurar a escolha do usuário.
  - [CA-2] Cada registro deve conter (ID-venda, usuário que realizou a venda, data e hora, total, custo, lucro, métodos de pagamento, e status informando se foi devolvido).
  - [CA-3] Por padrão deve ser listado o registro do mês, porém com a possibilidade do usuário descrever um intervalo de data, ou selecionar dentro das opções (diário, semanal, mensal, trimestral, semestral, anual).
  - [CA-4] Cada registro deve ser selecionável, para executar a função desejada.
  - [CA-5] A visualização da tabela deve ser listada por padrão em ordem de data, porém também possível de ordenar por id da venda, usuário, data, total, custo ou lucro. De forma crescente ou decrescente.

**VEN002 - Buscar registros de vendas**
- História de Usuário: Eu como gerente, quero realizar pesquisas mais específicas no módulo de vendas, pois assim posso encontrar registros específicos por meio de outros dados.
- Critérios de Aceite:
  - [CA-1] A busca deve ser possível por cada elemento da tabela de registros (ID-venda, usuário que realizou a venda, total, custo, lucro, métodos de pagamento, e status informando se foi devolvido).
  - [CA-2] Resultados devem ser paginados em páginas de 30 itens.
  - [CA-3] Filtros por intervalo de data (diário, semanal, mensal, trimestral, semestral, anual).

**VEN003 - Excluir venda**
- História de Usuário: Eu como gerente, quero excluir registros que eu julgue desnecessários.
- Critérios de Aceite:
  - [CA-1] Ao processar uma exclusão de registro, o gerente receberá uma notificação perguntando se tem certeza.
  - [CA-2] Mensagem de confirmação com confirmação de remoção.

**VEN004 - Devolver venda**
- História de Usuário: Eu como gerente, quero a funcionalidade devolução, para situações em que o cliente deseja realizar a devolução do produto comprado.
- Critérios de Aceite:
  - [CA-1] Ao processar uma devolução, o gerente receberá uma notificação perguntando se tem certeza.
  - [CA-2] A nota de devolução deve conter itens, quantidades, total, meios de pagamento e momento da compra.
  - [CA-3] Taxa da maquininha deve ser calculada e registrada (BR-03).

**VEN005 - Emitir nota não fiscal**
- História de Usuário: Eu como gerente, desejo emitir uma nota não fiscal para o cliente, pois assim em eventuais transtornos o cliente pode comprovar que realizou a compra em meu mercado.
- Critérios de Aceite:
  - [CA-1] A nota deve ter os itens comprados, suas quantidades, total, meios de pagamento e momento da compra.
  - [CA-2] A nota deve ser gerada automaticamente após a conclusão da venda (PDV001).

## Módulo: PDV

### Épico: PDV

**PDV001 - Processo de venda**
- História de Usuário: Eu como funcionária do caixa, desejo que o PDV siga os seguintes passos: Inserir código do produto, inserir quantidade, finalizar etapa de adicionar produtos, inserir forma(s) de pagamento, finalizar compra. O sistema deve exigir fechamento do caixa no início do dia, caso não tenha sido fechado anteriormente. Durante todo o processo de PDV deve haver os seguintes atalhos e funcionalidades: Caixa [F4], Alterar item [Ctrl + A], Deletar item [Ctrl + Del], Desconto [F10], Acréscimo [+], Consultar produto [Ctrl + P], Vendas [F7]. Para compras feitas pela maquininha, deve ser calculada sua taxa.
- Critérios de Aceite:
  - [CA-1] Inserção de código de barras via scanner HID (emulação digitação automática).
  - [CA-2] Produto encontrado → preencher automaticamente nome, valor unitário e quantidade padrão (1 ou peso se fracionado).
  - [CA-3] Produto não encontrado → mensagem: "Produto não cadastrado" + foco no campo de inserção.
  - [CA-4] Processo de PDV deve ser interrompido se o caixa não estiver aberto (BR-01).
  - [CA-5] Taxa da maquininha calculada e exibida no final.
  - Prioridade: P0 | Esforço: 2 SP

**PDV002 - Beeper do código do produto**
- História de Usuário: Eu como funcionária do caixa, desejo beepar os códigos do produto, e assim preencher automaticamente a identificação do produto durante o processo.
- Critérios de Aceite:
  - [CA-1] Scanner de código de barras detecta e converte para ID interno.
  - [CA-2] Produto não encontrado → mensagem de erro com código de erro.
  - [CA-3] Produto encontrado → carregar nome, valor, quantidade padrão.
  - Prioridade: P0 | Esforço: 2 SP

**PDV003 - Incluir automaticamente peso do produto**
- História de Usuário: Eu como funcionária do caixa, desejo que o peso dos produtos seja automaticamente preenchido no campo quantidade através da balança.
- Critérios de Aceite:
  - [CA-1] Integração com balança conectada via USB/serial (modelo: ________).
  - [CA-2] Ao detectar produto fracionado → solicitar peso na balança → preencher campo quantidade.
  - [CA-3] Peso > estoque disponível → bloqueio com mensagem clara.
  - Prioridade: P2 | Esforço: 1 SP

**PDV004 - Emitir nota não fiscal**
- História de Usuário: Eu como gerente, desejo que ao final do atendimento seja emitida uma nota não fiscal para o cliente, pois assim em eventuais transtornos o cliente pode comprovar que realizou a compra em meu mercado.
- Critérios de Aceite:
  - [CA-1] A nota deve ter os itens comprados, suas quantidades, total, meios de pagamento e momento da compra.
  - [CA-2] Emissão automática após conclusão do PDV001 (referencia VEN005).
  - Prioridade: P1 | Esforço: 2 SP

---

## Modelo de Dados

| Entidade | Campos Principais |
|----------|-------------------|
| **Produto** | `CodigoBarras`, `Nome`, `Fracionado`, `QuantidadeAtual`, `QuantidadeMin` |
| **Venda** | `Id`, `DataVenda`, `ValorTotal`, `Desconto`, `Acrescimo`, `Itens` |
| **ItemVenda** | `Produto`, `Quantidade`, `Subtotal` |
| **PagamentoVenda** | `TipoPagamento`, `Valor`, `TaxaMaquininha` |

---

## Requisitos Não-Funcionais

### Venda-NF
- [NF-01] Paginção de 30 itens renderiza em < 2s (10k registros).
- [NF-02] Dados sensíveis (CPF do cliente) criptografados em repouso.
- [NF-03] PDF de nota não fiscal gerado via `PrintDocument` (ReciboService).

### PDV-NF
- [NF-01] Tempo máximo entre leitura de código e exibição: < 1s.
- [NF-02] Caixa deve bloquear venda se não houver abertura diária.
- [NF-03] Taxa da maquininha recalculada e armazenada em `PagamentoVenda.TaxaMaquininha`.

---

## Regras de Negócio (BR)

- **BR-01**: Caixa aberto/fechado obrigatoriamente por dia (BR-04).
- **BR-02**: Admin notificado sobre produtos com validade próxima.
- **BR-03**: Taxa da maquininha calculada e aplicada em compras.
- **BR-04**: Caixa aberto/fechado diariamente.
- **BR-05**: Pelo menos 1 administrador obrigatório no sistema.

---

## Prioridade

| Requisito | Prioridade | Esforço |
|-----------|------------|----------|
| PDV001 | P0 | 2 SP |
| PDV002 | P0 | 2 SP |
| VEN001 | P1 | 3 SP |
| VEN004 | P1 | 2 SP |
| PDV004 | P1 | 2 SP |
| VEN002 | P2 | 1 SP |
| PDV003 | P2 | 1 SP |
| VEN003 | P3 | 1 SP |
| EST001 | P1 | 2 SP |
| EST002 | P1 | 2 SP |
| EST003 | P2 | 1 SP |
| EST004 | P2 | 1 SP |
| EST005 | P3 | 1 SP |
| CTV001 | P1 | 2 SP |
| CTV002 | P2 | 1 SP |
| FOR001 | P1 | 2 SP |
| FOR002 | P2 | 1 SP |
| FOR003 | P1 | 2 SP |
| FOR004 | P2 | 1 SP |
| FOR005 | P3 | 1 SP |
| LNC001 | P1 | 2 SP |
| LNC002 | P1 | 3 SP |
| LNC003 | P2 | 2 SP |
| LNC004 | P3 | 1 SP |
| LNC005 | P2 | 1 SP |
| CON001 | P1 | 1 SP |
| CON002 | P1 | 1 SP |
| CON003 | P2 | 1 SP |
| CON004 | P2 | 2 SP |
| CON005 | P2 | 1 SP |
| DAS001 | P1 | 3 SP |
| DAS002 | P2 | 2 SP |
| DAS003 | P2 | 3 SP |
| DAS004 | P2 | 2 SP |
| REL001 | P1 | 2 SP |
| REL002 | P2 | 2 SP |
| REL003 | P2 | 1 SP |
| REL004 | P2 | 1 SP |
| REL005 | P2 | 2 SP |
| REL006 | P2 | 2 SP |
| REL007 | P2 | 1 SP |
| REL008 | P1 | 2 SP |
| USU001 | P0 | 2 SP |
| USU002 | P1 | 1 SP |
| USU003 | P1 | 2 SP |
| USU004 | P2 | 1 SP |
| USU005 | P2 | 1 SP |
| INF001 | P1 | 3 SP |
| INF002 | P0 | 2 SP |
| INF003 | P0 | 2 SP |

---

## Módulo: Fornecedores

### Épico: Fornecedores

**FOR001 - Listar fornecedores**
- História de Usuário: Eu como gerente, desejo realizar buscas mais específicas de fornecedores.
- Critérios de Aceite:
  - [CA-1] A listagem deve conter paginação inicial de 30, porém com opção de configurar a escolha do usuário.
  - [CA-2] Cada registro deve conter (Código do fornecedor, Nome, endereço, telefone).
  - [CA-3] Cada registro deve ser selecionável, para executar a função desejada.
  - [CA-4] A visualização da tabela deve ser listada por padrão em ordem alfabética, porém também possível de ordenar por id do fornecedor, nome, endereço ou telefone. De forma crescente ou decrescente.
  - Prioridade: P1 | Esforço: 2 SP

**FOR002 - Buscar fornecedores**
- História de Usuário: Eu como gerente, desejo buscar de forma mais objetiva meus fornecedores.
- Critérios de Aceite:
  - [CA-1] A busca deve ser possível por cada elemento da tabela (Código do fornecedor, Nome, endereço, telefone).
  - [CA-2] Resultados paginados em páginas de 30 itens.
  - Prioridade: P2 | Esforço: 1 SP

**FOR003 - Adicionar fornecedor**
- História de Usuário: Desejo a opção de adicionar fornecedor, pois posso comprar um mesmo produto de dois ou mais fornecedores iguais, e assim posso diferenciar de quem estou comprando.
- Critérios de Aceite:
  - [CA-1] Os campos para adicionar fornecedores são: código do fornecedor, nome, telefone, endereço, email, site, cnpj, descrição.
  - [CA-2] Validação de campos obrigatórios antes de salvar.
  - Prioridade: P1 | Esforço: 2 SP

**FOR004 - Editar Fornecedor**
- História de Usuário: Desejo como gerente, modificar os dados dos meus fornecedores, para quaisquer eventuais mudanças.
- Critérios de Aceite:
  - [CA-1] Os campos para atualizar fornecedores são: código do fornecedor, nome, telefone, endereço, email, site, cnpj, descrição.
  - [CA-2] Confirmação de alteração antes de salvar.
  - Prioridade: P2 | Esforço: 1 SP

**FOR005 - Excluir Fornecedor**
- História de Usuário: Como gerente, desejo excluir fornecedores que eu não possua mais vínculo.
- Critérios de Aceite:
  - [CA-1] Ao excluir um ou mais fornecedores, o usuário deve ser alertado se realmente deseja prosseguir com a exclusão.
  - [CA-2] Mensagem de confirmação com confirmação de remoção.
  - Prioridade: P3 | Esforço: 1 SP

---

## Módulo: Controle de Caixa

### Épico: Controle de Caixa

**CTC001 - Listar Lançamentos**
- História de Usuário: Eu como gerente, quero que ao abrir o módulo de Controle de Caixa, seja exibido uma tabela com todos os registros, pois assim de forma ágil já tenho um primeiro feedback visual eficiente.
- Critérios de Aceite:
  - [CA-1] Paginação inicial de 30, com opção de configurar a escolha do usuário.
  - [CA-2] Cada registro deve conter (Usuário, descrição, data, valor em sangria se houver, valor).
  - [CA-3] Cada registro selecionável para executar função desejada.
  - [CA-4] Visualização por padrão em ordem de data, possível ordenar pelas demais colunas.
  - Prioridade: P1 | Esforço: 2 SP

**CTC002 - Novo lançamento avulso**
- História de Usuário: Eu como gerente, quero inserir novas movimentações manualmente.
- Critérios de Aceite:
  - [CA-1] Campos: Valor e detalhes.
  - [CA-2] Registro vinculado ao usuário que realizou.
  - [CA-3] Validação: valor obrigatório, detalhes obrigatório.
  - Prioridade: P1 | Esforço: 1 SP

**CTC003 - Alterar lançamento**
- História de Usuário: Eu como gerente, quero atualizar minhas movimentações manualmente.
- Critérios de Aceite:
  - [CA-1] Campos atualizáveis: Valor e detalhes.
  - [CA-2] Confirmação de alteração antes de salvar.
  - Prioridade: P1 | Esforço: 1 SP

**CTC004 - Apagar lançamento**
- História de Usuário: Eu como gerente, desejo excluir meus lançamentos indesejados.
- Critérios de Aceite:
  - [CA-1] Caixa de alerta para confirmar exclusão.
  - [CA-2] Exclusão só para registros do usuário (ou admin).
  - [CA-3] Confirmar se realmente deseja excluir o(s) lançamento(s).
  - Prioridade: P2 | Esforço: 1 SP

**CTC005 - Apuração**
- História de Usuário: Eu como gerente, desejo realizar a apuração das movimentações realizadas diariamente, pois assim contesto se realmente está tudo certo com todas entradas e saídas.
- Critérios de Aceite:
  - [CA-1] Possível apurar um registro de cada vez.
  - [CA-2] Possível apurar todos de uma vez conforme seleção do usuário.
  - [CA-3] Apuração deve considerar sangrias (CTC006) e lançamentos avulsos (CTC002).
  - [CA-4] Só deve ser executada se caixa estiver aberto (BR-01).
  - Prioridade: P1 | Esforço: 2 SP

**CTC006 - Sangria**
- História de Usuário: Eu como gerente, desejo realizar a sangria dos caixas quando eu desejar, pois assim posso retirar o excesso de dinheiro e evitar eventuais problemas.
- Critérios de Aceite:
  - [CA-1] Ação de sangria registrada no histórico (CTC001).
  - [CA-2] Valor da sangria registrado e visível no lançamento.
  - [CA-3] Só deve ser executada se caixa estiver aberto (BR-01).
  - [CA-4] Sangria deve ser considerada na apuração (CTC005).
  - Prioridade: P0 | Esforço: 2 SP

**CTC007 - Exportar registro**
- História de Usuário: Eu como gerente, desejo imprimir algum dos registros presentes no sistema, pois assim posso acompanhar este registro comigo até o caixa.
- Critérios de Aceite:
  - [CA-1] Exportação feita em formato PDF.
  - [CA-2] PDF deve conter: descrição, data, valor, usuário, status de sangria.
  - [CA-3] Exportação deve ser feita a partir do registro selecionado (CTC001).
  - Prioridade: P2 | Esforço: 1 SP

---

## Módulo: Estoque

### Épico: Estoque

**EST001 - Listar Produtos**
- História de Usuário: Eu como gerente, quero que ao abrir o módulo de Estoque, seja exibido uma tabela com todos os registros de produtos.
- Critérios de Aceite:
  - [CA-1] A listagem deve conter paginação inicial de 30, porém com opção de configurar a escolha do usuário.
  - [CA-2] Cada registro deve conter (Código do produto, Nome, Custo, Lucro, Sugestão de valor venda, Valor de venda, Quantidade mínima, Quantidade atual, Fracionado ou Inteiro, status se inativo).
  - [CA-3] Cada registro deve ser selecionável, para executar a função desejada.
  - [CA-4] A visualização da tabela deve ser listada por padrão em ordem alfabética, porém também possível de ordenar por id, nome, custo, lucro, valor de venda, quantidade mínima, quantidade atual, fracionados, controle de validade, status se inativo. De forma crescente ou decrescente.
  - Prioridade: P1 | Esforço: 2 SP

**EST002 - Adicionar Produto**
- História de Usuário: Eu como gerente, quero obviamente a opção de adicionar produtos, para assim fazer o gerenciamento dos meus itens físicos.
- Critérios de Aceite:
  - [CA-1] Os campos para adicionar produtos são: Codigo, Nome, Custo, Lucro %, Preço de venda, Estoque mínimo, Produto pesável ou não.
  - [CA-2] Sugestão de preço baseado no custo e lucro automaticamente.
  - [CA-3] Validação dos campos obrigatórios antes de salvar.
  - Prioridade: P1 | Esforço: 2 SP

**EST003 - Atualizar Produto**
- História de Usuário: Eu como gerente, quero atualizar meus produtos, pois assim posso gerenciar prováveis erros ou mudanças.
- Critérios de Aceite:
  - [CA-1] Os campos para atualizar produtos são: Codigo, Nome, Custo, Lucro %, Preço de venda, Estoque mínimo, Produto pesável ou não.
  - [CA-2] Possibilidade de inativar / ativar produto.
  - [CA-3] Confirmação de alteração antes de salvar.
  - Prioridade: P2 | Esforço: 1 SP

**EST004 - Buscar Produtos**
- História de Usuário: Eu como gerente, desejo realizar buscas mais específicas por produtos, pois terei muitos produtos em minha base de dados, então necessito disso.
- Critérios de Aceite:
  - [CA-1] Os campos para buscar produtos são: Codigo, Nome, Custo, Lucro %, Preço de venda, Estoque mínimo, Produto pesável ou não.
  - [CA-2] Resultados paginados em páginas de 30 itens.
  - [CA-3] Possibilidade de ordenar e filtrar resultados.
  - Prioridade: P2 | Esforço: 1 SP

**EST005 - Excluir Produtos**
- História de Usuário: Eu como gerente, desejo excluir meus produtos indesejados, pois provavelmente ocorrerá de eu não vender mais algum tipo de produto.
- Critérios de Aceite:
  - [CA-1] Deve haver uma caixa de alerta para o usuário confirmar se realmente deseja excluir o(s) produto(s).
  - [CA-2] Mensagem de confirmação com confirmação de remoção.
  - Prioridade: P3 | Esforço: 1 SP

---

## Módulo: Controle de validade

### Épico: Controle de validade

**CTV001 - Listar lotes de validade**
- História de Usuário: Eu como gerente, desejo visualizar todos os meus lotes de validade.
- Critérios de Aceite:
  - [CA-1] Os campos para a tabela de listagem devem ser: Codigo do produto, nome do produto, Quantidade de lotes por produto, data dos produtos mais próximo a vencer.
  - [CA-2] A listagem deve conter paginação inicial de 30, porém com opção de configurar a escolha do usuário.
  - [CA-3] A visualização da tabela deve ser listada por padrão em ordem do mais próximo de vencer, porém também possível de ordenar por Código do produto, nome do produto, Quantidade de lotes por produto. De forma crescente ou decrescente.
  - [CA-4] Cada registro deve ser selecionável, para executar a função desejada.
  - Prioridade: P1 | Esforço: 2 SP

**CTV002 - Buscar lote**
- História de Usuário: Desejo como gerente, realizar buscas mais filtradas para o módulo de controle de validade, pois quando estiver com uma gigantesca base de dados, posso buscar por algo específico mais facilmente.
- Critérios de Aceite:
  - [CA-1] A busca e filtro pode ser por Id do produto, nome do produto, quantidade de lotes, próximo quantidade a vencer, e intervalo de data para vencimento.
  - [CA-2] Resultados paginados.
  - Prioridade: P2 | Esforço: 1 SP

---

## Módulo: Lançamento de Compras

### Épico: Lançamento de Compras

**LNC001 - Listar registros de compras**
- História de Usuário: Eu como gerente, desejo visualizar todos os meus Registros de compras.
- Critérios de Aceite:
  - [CA-1] Os campos para a tabela de listagem devem ser: código da compra, Fornecedor, custo, acréscimo, decrescimo, total, a pagar.
  - [CA-2] A listagem deve conter paginação inicial de 30, porém com opção de configurar a escolha do usuário.
  - [CA-3] A visualização da tabela deve ser listada por padrão em ordem do código decrescente, porém também possível de ordenar por fornecedor, custo, acréscimo, decrescimo, total, a pagar. De forma crescente ou decrescente.
  - [CA-4] Cada registro deve ser selecionável, para executar a função desejada.
  - Prioridade: P1 | Esforço: 2 SP

**LNC002 - Lançar nova compra**
- História de Usuário: Desejo a opção de lançar nova compra, para um fácil gerenciamento geral de meu estoque e financeiro.
- Critérios de Aceite:
  - [CA-1] O lançamento deve ser separado por seções: Dados de Cabeçalho (Fornecedor, data e hora, acréscimo, desconto); Itens (produto, quantidade, valor da compra, total); Controle de validade para produtos (vencimento do lote, identificador do lote, quantidade de produtos do lote); Financeiro (valor a pagar, data do pagamento).
  - [CA-2] Validação de campos obrigatórios antes de salvar.
  - Prioridade: P1 | Esforço: 3 SP

**LNC003 - Atualizar compra**
- História de Usuário: Como gerente, desejo atualizar uma compra existente.
- Critérios de Aceite:
  - [CA-1] Todas as especificações de lançar novas compras devem ser atualizáveis, seguindo o mesmo processo (Cabeçalho, Itens, Controle de validade, Financeiro).
  - [CA-2] Confirmação de alteração antes de salvar.
  - Prioridade: P2 | Esforço: 2 SP

**LNC004 - Excluir compra**
- História de Usuário: Como gerente, desejo excluir Compras que eu julgar incorretas.
- Critérios de Aceite:
  - [CA-1] Ao excluir uma ou mais compras, o usuário deve ser alertado se realmente deseja prosseguir com a exclusão.
  - [CA-2] Mensagem de confirmação com confirmação de remoção.
  - Prioridade: P3 | Esforço: 1 SP

**LNC005 - Buscar compra**
- História de Usuário: Como gerente, desejo filtrar mais especificamente minhas compras, pois assim posso filtrar melhor dentre as milhares de compras registradas.
- Critérios de Aceite:
  - [CA-1] A busca deve ser possível por cada elemento da tabela (código da compra, Fornecedor, custo, acréscimo, decrescimo, total, a pagar).
  - [CA-2] Resultados paginados em páginas de 30 itens.
  - Prioridade: P2 | Esforço: 1 SP

---

## Módulo: Contas a pagar

### Épico: Contas a pagar

**CON001 - Adicionar Conta**
- História de Usuário: Eu como gerente, desejo a opção de adicionar Conta, pois posso gerir as finanças do mercado de forma mais eficiente.
- Critérios de Aceite:
  - [CA-1] Os campos para adicionar conta são: Valor, Vencimento, Fornecedor, Detalhes, Ocorrencias.
  - [CA-2] Validação de campos obrigatórios antes de salvar.
  - Prioridade: P1 | Esforço: 1 SP

**CON002 - Alterar Conta**
- História de Usuário: Desejo como gerente, modificar os dados das minhas contas, para quaisquer eventuais mudanças.
- Critérios de Aceite:
  - [CA-1] Os campos para atualizar contas são: Valor, Vencimento, Fornecedor, Detalhes, Ocorrencias.
  - [CA-2] Confirmação de alteração antes de salvar.
  - Prioridade: P1 | Esforço: 1 SP

**CON003 - Excluir Conta**
- História de Usuário: Como gerente, desejo excluir Contas que eu julgar incorretas.
- Critérios de Aceite:
  - [CA-1] Ao excluir uma ou mais Contas, o usuário deve ser alertado se realmente deseja prosseguir com a exclusão.
  - [CA-2] Mensagem de confirmação com confirmação de remoção.
  - Prioridade: P2 | Esforço: 1 SP

**CON004 - Extrato**
- História de Usuário: Como gerente, desejo retirar extrato de minhas movimentações, seguindo o período que eu desejar.
- Critérios de Aceite:
  - [CA-1] Possibilidade de selecionar período para geração do extrato.
  - [CA-2] Extrato deve conter as movimentações (contas) do período selecionado.
  - Prioridade: P2 | Esforço: 2 SP

**CON005 - Detalhes da conta**
- História de Usuário: Como gerente, necessito visualizar informações mais detalhadas de uma conta, pois assim posso recordar detalhes como compras associadas a este débito.
- Critérios de Aceite:
  - [CA-1] Os detalhes associados à conta devem ser a lista de produtos comprados.
  - [CA-2] Visualização completa dos itens vinculados à conta selecionada.
  - Prioridade: P2 | Esforço: 1 SP

---

## Módulo: Dashboard

### Épico: Dashboard

**DAS001 - Visão Geral / Saúde do Negócio**
- História de Usuário: Eu como gerente, preciso de um painel de gestão geral do meu sistema, pois assim posso ter uma visão estratégica para melhores decisões.
- Critérios de Aceite:
  - [CA-1] Deve haver informação de Faturamento bruto diário/mensal.
  - [CA-2] Deve haver informação de Ticket médio = (Faturamento / Número de compras).
  - [CA-3] Deve haver informação de Quantidade de transações.
  - [CA-4] Deve haver informação de Margem de lucro.
  - [CA-5] Deve haver informação de Formas de pagamento mais utilizadas (Gráfico pizza).
  - Prioridade: P1 | Esforço: 3 SP

**DAS002 - Gestão de Estoque e Prevenção de Perdas**
- História de Usuário: Eu como gerente, preciso de informações gerais do meu estoque, pois assim posso ter uma visão estratégica para melhores decisões.
- Critérios de Aceite:
  - [CA-1] Deve haver informação de Produtos com baixo estoque.
  - [CA-2] Deve haver informação de Giro de estoque, produtos que mais saem e menos saem.
  - [CA-3] Deve haver informação de validades próximas.
  - Prioridade: P2 | Esforço: 2 SP

**DAS003 - Desempenho de Vendas e Curva ABC**
- História de Usuário: Eu como gerente, preciso de informações referente ao desempenho e curva ABC, para entender quais produtos realmente mantêm o mercado vivo.
- Critérios de Aceite:
  - [CA-1] A curva ABC deve ser separada: os 20% dos produtos que geram 80% do faturamento; produtos de importância média; produtos de baixo faturamento.
  - [CA-2] Top 10 produtos mais vendidos, tanto em volume quanto em valor financeiro.
  - Prioridade: P2 | Esforço: 3 SP

**DAS004 - Monitoramento de Frente de Caixa**
- História de Usuário: Eu como gerente, preciso de informações em tempo real referente à frente de Caixas, para ter uma visão estratégica para melhores decisões.
- Critérios de Aceite:
  - [CA-1] Status dos caixas em tempo real (fechado, ativo).
  - [CA-2] Velocidade de atendimento por caixa.
  - [CA-3] Volume de cancelamentos / Devoluções.
  - Prioridade: P2 | Esforço: 2 SP

---

## Módulo: Relatórios

### Épico: Relatórios

**REL001 - Relatório de Vendas**
- História de Usuário: Eu como gerente, preciso de relatórios de vendas, para ter uma boa visão estratégica e tomar melhores decisões.
- Critérios de Aceite:
  - [CA-1] Opções de relatório: Todas as vendas (sem os itens); Venda selecionada (e seus itens); Itens vendidos (ordenados por quantidade); Itens vendidos (ordenados por nome); Curva ABC de produtos (por faturamento); Curva ABC de produtos (por lucro); Vendas por funcionários; Vendas removidas ou com devolução.
  - Prioridade: P1 | Esforço: 2 SP

**REL002 - Relatório de Estoque**
- História de Usuário: Eu como gerente, preciso de relatórios de estoque, para ter uma boa visão estratégica e tomar melhores decisões.
- Critérios de Aceite:
  - [CA-1] Opções de relatório: Menor ou igual ao estoque mínimo; Menor ou igual a zero; Diferente de zero; Menor que zero; Maior que zero; Todos os produtos; Produtos pesáveis; Histórico de vendas; Histórico de fornecedores.
  - Prioridade: P2 | Esforço: 2 SP

**REL003 - Relatório de Controle de validade**
- História de Usuário: Eu como gerente, necessito de relatórios de validade, para gerenciar melhor meus produtos que estão próximo da validade.
- Critérios de Aceite:
  - [CA-1] Filtro por períodos de tempo.
  - [CA-2] Listagem por data ou produto.
  - Prioridade: P2 | Esforço: 1 SP

**REL004 - Relatório de Fornecedor**
- História de Usuário: Eu como gerente, desejo exportar a listagem dos meus fornecedores.
- Critérios de Aceite:
  - [CA-1] Exportação da listagem completa de fornecedores.
  - Prioridade: P2 | Esforço: 1 SP

**REL005 - Relatório de Lançamento de compras**
- História de Usuário: Eu como gerente, preciso de relatórios de compras, para ter uma boa visão estratégica e tomar melhores decisões.
- Critérios de Aceite:
  - [CA-1] Opções de relatório: Todas as compras (sem os itens); Compra selecionada (e seus itens); Itens comprados (Custo min, med e max); Compras por fornecedor.
  - Prioridade: P2 | Esforço: 2 SP

**REL006 - Relatório de Contas a pagar**
- História de Usuário: Eu como gerente, preciso de relatórios de Contas a Pagar, para ter uma boa visão estratégica e tomar melhores decisões.
- Critérios de Aceite:
  - [CA-1] Opções de relatório: Todos; Todos os atrasados; Todos em dia; Por Fornecedor; Por Vencimento; Por Intervalo.
  - Prioridade: P2 | Esforço: 2 SP

**REL007 - Exportar Dashboard**
- História de Usuário: Eu como gerente, desejo exportar meu dashboard.
- Critérios de Aceite:
  - [CA-1] Exportação do painel/dashboard para formato de relatório.
  - Prioridade: P2 | Esforço: 1 SP

**REL008 - Relatório de Caixa**
- História de Usuário: Eu como gerente, preciso de relatórios de Controle de Caixa, para ter uma boa visão estratégica e tomar melhores decisões.
- Critérios de Aceite:
  - [CA-1] Movimentos exportáveis conforme seleção do usuário: Em aberto, Anual, Mensal, Diário, Apurados.
  - Prioridade: P1 | Esforço: 2 SP

---

## Módulo: Usuários

### Épico: Usuários

**USU001 - Autenticação**
- História de Usuário: Eu como gerente, desejo que meu sistema possua login e logout, para melhor gerenciamento das funções de cada pessoa, e maior segurança.
- Critérios de Aceite:
  - [CA-1] Para efetuar login, deve ser informado usuário e senha.
  - [CA-2] Funcionalidade de logout disponível.
  - Prioridade: P0 | Esforço: 2 SP

**USU002 - Listar usuários**
- História de Usuário: Eu como gerente, desejo visualizar todos os registros de usuários presentes em meu sistema.
- Critérios de Aceite:
  - [CA-1] Os dados mostrados em cada registro devem ser: Nome, e status.
  - [CA-2] Registros selecionáveis para executar função desejada.
  - Prioridade: P1 | Esforço: 1 SP

**USU003 - Adicionar usuário**
- História de Usuário: Eu como gerente, desejo adicionar novos usuários em meu sistema, pois meu empreendimento conforme o tempo evoluirá a quantidade de funcionários.
- Critérios de Aceite:
  - [CA-1] Os dados para cadastro são: Nome, Tipo, Senha.
  - [CA-2] Nome e senha devem ser únicos.
  - Prioridade: P1 | Esforço: 2 SP

**USU004 - Atualizar usuário**
- História de Usuário: Eu como gerente, preciso alterar informações de usuários do sistema, para eventuais perdas de senhas, ou troca de autoridade no sistema.
- Critérios de Aceite:
  - [CA-1] Os campos atualizáveis são: Nome, Tipo, Senha.
  - [CA-2] Confirmação de alteração antes de salvar.
  - Prioridade: P2 | Esforço: 1 SP

**USU005 - Excluir usuário**
- História de Usuário: Eu como gerente, quero excluir usuários eventualmente, pois caso um funcionário não pertença mais a empresa, quero excluir sua conta.
- Critérios de Aceite:
  - [CA-1] Deve aparecer uma caixa de diálogo se certificando se realmente deseja realizar a exclusão do usuário.
  - [CA-2] O sistema nunca deve permitir excluir todos os usuários administradores.
  - Prioridade: P2 | Esforço: 1 SP

---

## Módulo: Infraestrutura

### Épico: Infraestrutura

**INF001 - Configuração do Banco de Dados Centralizado (Escritório)**
- História de Usuário: Eu como gerente, desejo que meu PC comporte o servidor principal do sistema, para melhor eficiência geral.
- Critérios de Aceite:
  - [CA-1] Configuração do servidor centralizado no PC do escritório.
  - [CA-2] Eficiência no acesso e gerenciamento do banco de dados.
  - Prioridade: P1 | Esforço: 3 SP

**INF002 - Comunicação e Acesso do Cliente (Caixa)**
- História de Usuário: Como operador de caixa, eu quero que o sistema se conecte automaticamente ao banco de dados do escritório ao ser iniciado, para que eu possa realizar vendas sem precisar configurar endereços de IP manualmente todos os dias.
- Critérios de Aceite:
  - [CA-1] Conexão automática ao banco de dados do escritório ao iniciar o sistema.
  - [CA-2] Sem necessidade de configurar endereços de IP manualmente diariamente.
  - Prioridade: P0 | Esforço: 2 SP

**INF003 - Backup**
- História de Usuário: Eu como gerente, desejo sempre ter uma cópia dos dados de meu comércio, para assim ter segurança contra qualquer imprevisto.
- Critérios de Aceite:
  - [CA-1] Backup pode ser por Google Drive ou Local.
  - [CA-2] Deve haver opção de backup automático ou manual.
  - Prioridade: P0 | Esforço: 2 SP