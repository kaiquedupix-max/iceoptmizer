# ice optimizer

Interface Windows e integração por **Maciota**. Versão **2.5 — Edição Glacial**: animações com temporização de alta resolução e alvo de 60 FPS, brilho que acompanha o cursor e cartões responsivos, navegação lateral, inputs com foco animado, elevação administrativa automática, ambiente pré-configurado, logout, neve, progresso com cubo de gelo, descrições das 160 ações, seleção por categoria, histórico local, conta e perfil de licença.

Marque os cartões ou use **Marcar categoria**, depois **Revisar e executar**. A revisão permite desmarcar combinações opostas. Reiniciar, desligar, encerrar Explorer e verificar disco precisam de execução separada. O ponto de restauração selecionado vem primeiro. Todos os downloads são preparados antes da primeira mudança. Durante a execução, o aplicativo bloqueia os cliques e mostra o progresso em uma tela gelada; **Esc** solicita uma parada segura depois da ação atual, sem interromper um comando do Windows no meio. A fila continua quando uma ação informa erro, mostra o resultado individual e limpa a seleção ao terminar.

O portal em `portal/` inclui página comercial, painel administrativo, API de contas e licenças e checkout Stripe. Configure `ADMIN_PASSWORD`, `SESSION_SECRET`, `DATA_PATH=/data/licenses.json`, `BASE_URL`, `STRIPE_SECRET_KEY` e `STRIPE_WEBHOOK_SECRET` no Railway e monte um volume em `/data`. No Stripe, cadastre o webhook `https://ice-optimizer-web-production.up.railway.app/api/stripe/webhook` para o evento `checkout.session.completed`. A compra mensal de R$ 29,90 gera uma licença de 30 dias; o plano completo de R$ 100,00 inclui acesso permanente e atendimento profissional personalizado por AnyDesk. A página de sucesso libera a key e o download depois da confirmação do pagamento. O checkout permanece indisponível enquanto as credenciais Stripe não estiverem configuradas.

A key aparece completa na criação administrativa e na entrega do pedido. Ela cria uma conta com usuário e senha, começa a valer no cadastro e fica vinculada ao primeiro computador por uma impressão de hardware composta por UUID do sistema, serial da BIOS, placa-mãe e processador. Keys e tokens são armazenados como hashes, senhas usam scrypt com salt individual e a key entregue pelo checkout fica criptografada em repouso até ser consultada pela sessão de pagamento.

## Usar

1. Baixe `ice-optimizer.exe` em `downloads` neste repositório.
2. Crie sua conta com uma key ainda não utilizada ou entre com seu usuário e senha.
3. Abra com **Executar como administrador** para aplicar ajustes.
4. Escolha a categoria ou busque a ação (Ctrl+F). Marque as ações desejadas, leia seus efeitos em **Ver detalhes** e revise o plano antes de confirmar.
5. O aplicativo baixa apenas o script selecionado e suas dependências de `kaiquedupix-max/iceoptmizer`, confere o SHA-256 embutido no executável e executa a ação.
6. Consulte o tempo restante ou encerre a sessão em **Minha conta** e acompanhe o **Histórico**. Cada plano mantém `plano.log` e `resultado.json`; cada ação tem seu `execution.log`. O aplicativo organiza esses arquivos automaticamente na pasta local do Ice Optimizer.

A pasta de trabalho armazena downloads e backups. Os comandos afetam o Windows em execução, não uma instalação Windows em outra unidade. A opção de jogos ajusta prioridades dos executáveis conhecidos; não precisa selecionar a pasta do jogo.

O pacote `ice-optimizer-offline.zip` permite executar sem internet: extraia tudo no mesmo local. O aplicativo identifica os scripts locais automaticamente. Nunca mova apenas o executável quando usar esse modo.

## Limites dos scripts

As ações foram separadas do menu de texto e receberam uma interface; isso não comprova ganho de desempenho. Algumas desativam impressão, biometria, recursos Xbox, virtualização e proteções do Windows ou removem aplicativos. Confira o efeito indicado. Faça um ponto de restauração antes de alterações. Criar um ponto depende da Proteção do Sistema e pode falhar.

Código de saída zero do `.bat` não garante que todos os seus comandos tiveram sucesso: há scripts legados que suprimem mensagens e ignoram falhas intermediárias. A interface informa **encerrado**, e pede revisão do registro. Algumas ferramentas abrem uma janela própria e continuam depois do script. Não há reversão universal automática; use as ações de reversão existentes ou a recuperação do Windows. Scripts que pedem entrada no console recebem entrada fechada e usam o comportamento padrão do comando.

O executável não está assinado digitalmente. Validação feita sem aplicar otimizações ao computador de desenvolvimento; os ajustes precisam de testes em máquina virtual Windows antes de distribuição comercial.

## Desenvolvimento

- Windows com .NET Framework 4.x, PowerShell e Node.js (Node apenas para regenerar o catálogo e testes).
- `node tools/catalog.mjs`: separa as ações e atualiza hashes.
- `node --test tests/catalog.test.mjs`: verifica integridade, destinos e ausência de menus interativos nas ações.
- `powershell -NoProfile -ExecutionPolicy Bypass -File build.ps1`: gera EXE e ZIP em `dist`.
- `dist/ice-optimizer.exe --self-test dist/self-test.txt`: testa interface, filtros e validações sem executar ajustes.

O catálogo confiável é incorporado no EXE. Alterações nos scripts exigem regeneração e recompilação; divergências de hash impedem execução. `scripts/engine.bat` e `scripts/debloater.bat` preservam o material fornecido com nova marca para manutenção; o aplicativo usa `scripts/actions/`.

Veja `THIRD-PARTY-NOTICES.md` para os utilitários externos e restrições de redistribuição. Os créditos da interface e integração não transferem autoria dos componentes externos.
