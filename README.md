# ice optimizer

Interface Windows e integração por **Maciota**. Versão **2.0 — Edição Glacial**: identidade de gelo com logo vetorial, cartões personalizados, animações suaves, descrições das 160 ações, seleção por categoria, revisão de conflitos, execução sequencial e histórico local.

Marque os cartões ou use **Marcar categoria**, depois **Revisar e executar**. A revisão permite desmarcar combinações opostas. Reiniciar, desligar, encerrar Explorer e verificar disco precisam de execução separada. O ponto de restauração selecionado vem primeiro. Todos os downloads são preparados antes da primeira mudança. **Parar após esta ação** encerra a fila entre ações, sem matar um comando do Windows em andamento. Configurações permite reduzir movimento, escolher modo offline e decidir se a fila para após erro informado.

## Usar

1. Baixe `ice-optimizer.exe` em `downloads` neste repositório.
2. Abra com **Executar como administrador** para aplicar ajustes.
3. Escolha a categoria ou busque a ação (Ctrl+F). Marque as ações desejadas, leia seus efeitos em **Ver detalhes** e revise o plano antes de confirmar.
4. O aplicativo baixa apenas o script selecionado e suas dependências de `kaiquedupix-max/iceoptmizer`, confere o SHA-256 embutido no executável e executa a ação. Não precisa de conta, token, banco de dados ou Heroku.
5. Acompanhe o progresso e o **Histórico**. Cada plano mantém `plano.log` e `resultado.json`; cada ação tem seu `execution.log`. A pasta é escolhida em **Configurações**.

A pasta de trabalho armazena downloads e backups. Os comandos afetam o Windows em execução, não uma instalação Windows em outra unidade. A opção de jogos ajusta prioridades dos executáveis conhecidos; não precisa selecionar a pasta do jogo.

O pacote `ice-optimizer-offline.zip` permite executar sem internet: extraia tudo e marque **Usar scripts locais**. Nunca mova apenas o executável quando usar esse modo.

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
