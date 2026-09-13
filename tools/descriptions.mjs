import fs from 'node:fs';
const detail={
ping:['Renovar conexão e DNS','Limpa o cache DNS, libera e renova o endereço IP. Depois abre o DNS Jumper para escolher servidores DNS.','A rede será interrompida e uma ferramenta externa será aberta.'],
limparram:['Limpar memória em espera','Usa EmptyStandbyList para esvaziar listas de memória em espera, páginas modificadas e conjuntos de trabalho.','Aplicativos podem precisar recarregar dados; o efeito pode ser temporário.'],
restore:['Criar ponto de restauração','Solicita um ponto de restauração do Windows e exporta chaves do Explorer e da área de trabalho para a pasta Backup.','Depende da Proteção do Sistema. Não é um backup completo do computador.'],
opcao1:['Ajustar plano de energia','Duplica o plano Desempenho Máximo, ajusta a ociosidade da CPU no plano atual e abre o painel de energia.','O plano duplicado precisa ser escolhido no painel; o script mantém o plano atual ativo.'],
opcao2:['Reduzir efeitos visuais','Configura a aparência do Windows para desempenho e desativa transparências do sistema.','Muda a aparência do Windows e pode exigir nova sessão.'],
opcao3:['Limitar apps em segundo plano','Desativa a permissão global de execução em segundo plano para apps compatíveis e o rastreamento de programas.','Pode afetar atualizações e notificações de aplicativos.'],
opcao4:['Desativar serviços do sistema','Interrompe relatórios de erro, telemetria, biometria e o serviço de impressão. Configura esses serviços como desativados.','Impressão e autenticação biométrica podem parar de funcionar.'],
opcao5:['Reduzir captura da Game Bar','Desativa Game DVR, avisos e integração da Game Bar, além de ajustar o comportamento de tela cheia.','Gravações e recursos de captura do Xbox podem ficar indisponíveis.'],
desativarxbox:['Remover recursos Xbox','Remove pacotes Xbox do usuário e do provisionamento do Windows, desativa capturas e serviços relacionados.','Pode afetar Game Pass, login e salvamentos. Não equivale apenas a desligar serviços.'],
reverterxbox:['Reativar recursos Xbox','Restaura a inicialização manual dos serviços Xbox, tenta registrar a Game Bar instalada e reativa Game DVR.','Não garante recuperar pacotes já removidos; pode exigir reinstalação.'],
opcao7:['Desativar relatórios de erro','Desativa o Windows Error Reporting e interrompe seu serviço de envio de relatórios.','Reduz informações disponíveis para diagnosticar falhas.'],
opcao8:['Limitar coleta e sugestões','Aplica políticas de coleta de dados, publicidade e sugestões. Também restringe conexões a locais online do Windows Update.','Pode afetar atualizações. A eficácia das políticas depende da edição do Windows.'],
opcao9:['Desativar hibernação','Desativa a hibernação pelo powercfg, removendo a necessidade do arquivo usado para salvar a sessão em disco.','Também pode desativar a Inicialização Rápida.'],
desativarmemoria:['Desativar compressão de RAM','Desativa a compressão de memória do Windows pelo gerenciamento de memória MMAgent.','Pode aumentar o uso de RAM e de paginação; não garante ganho de desempenho.'],
ativarmemoria:['Ativar compressão de RAM','Ativa novamente a compressão de memória do Windows pelo MMAgent.','Reverte a desativação deste recurso; não restaura outras configurações.'],
opcao11:['Desativar indexação de busca','Interrompe o Windows Search e desativa sua inicialização automática.','Pesquisas de arquivos e de aplicativos podem ficar mais lentas ou incompletas.'],
opcao12:['Simplificar busca do Iniciar','Desativa resultados do Bing e sugestões de busca, remove consentimento da Cortana e oculta a caixa de pesquisa.','Altera a busca e a apresentação da barra de tarefas.'],
opcao13:['Desativar e remover Cortana','Aplica a política que desativa a Cortana e tenta remover seu pacote do usuário atual.','Só tem efeito quando esse componente ainda está instalado.'],
desativarsuper:['Desativar Prefetch e SysMain','Desativa o pré-carregamento Prefetch/Superfetch e interrompe o serviço SysMain.','Pode piorar a abertura de aplicativos, dependendo do hardware e do uso.'],
ativarsuper:['Reativar Prefetch e SysMain','Configura Prefetch e Superfetch no valor 3 e reativa o início automático do SysMain.','Restaura valores definidos pelo script; não recupera um backup anterior.'],
opcao15:['Ajustar prioridade de CPU e GPU','Modifica prioridades e categorias de agendamento multimídia para tarefas de jogos no Registro.','O resultado varia por jogo e sistema. Não aumenta a capacidade física do hardware.'],
otimizarforeground:['Priorizar primeiro plano','Altera a divisão de tempo da CPU e prioridades multimídia para favorecer aplicativos em primeiro plano.','Pode alterar a resposta de tarefas em segundo plano.'],
voltarforeground:['Restaurar prioridade de primeiro plano','Reaplica valores definidos no script para divisão de CPU e prioridades de jogos.','É uma restauração para valores fixos, não para suas configurações anteriores.'],
opcao17:['Desativar isolamento de núcleo','Desativa políticas de segurança baseada em virtualização e a inicialização do hipervisor.','Reduz proteções do Windows e pode afetar virtualização. Exige reinicialização.'],
debloater:['Remover aplicativos em lote','Remove diversos pacotes, incluindo Cortana, Office Hub, mapas, calendário, notícias e assistência rápida.','Pode remover apps usados por você; a reversão não é garantida.'],
reverterdebloater:['Registrar aplicativos novamente','Tenta registrar manifestos de aplicativos provisionados e instalados para todos os usuários.','Não baixa aplicativos ausentes e pode apresentar falhas parciais.'],
opcao19:['Ativar Modo de Jogo','Ativa as opções AutoGameModeEnabled e AllowAutoGameMode do Windows no usuário atual.','Permite que o Windows gerencie recursos durante jogos; não garante mais FPS.'],
opcao20:['Ajustar tela cheia','Configura o comportamento global de otimizações de tela cheia nas chaves GameConfigStore.','Pode melhorar ou piorar compatibilidade, latência e captura, conforme o jogo.'],
opcao21:['Desativar aceleração do mouse','Zera MouseSpeed e os limiares de aceleração de ponteiro no perfil do usuário.','Muda a sensação do ponteiro; jogos com entrada bruta podem ignorar o ajuste.'],
opcao22:['Abrir ISLC','Abre o Intelligent Standby List Cleaner para configurar a limpeza de memória em espera e a resolução do timer.','A configuração ocorre na janela do utilitário. Não aplica um perfil automaticamente.'],
opcao23:['Desativar VBS e HVCI','Desativa segurança baseada em virtualização, integridade de código protegida pelo hipervisor e início do hipervisor.','Reduz proteções contra código malicioso. Exige reinicialização e pode afetar VMs.'],
opcao24:['Desativar assistente de compatibilidade','Interrompe o serviço de compatibilidade de programas e desativa mecanismos e inventário AppCompat.','Programas antigos podem perder correções automáticas de compatibilidade.'],
opcao25:['Fechar Windows Explorer','Encerra à força o processo explorer.exe, incluindo a área de trabalho e a barra de tarefas.','Execute separadamente. Use Iniciar Explorer para voltar à interface do Windows.'],
opcao26:['Iniciar Windows Explorer','Inicia explorer.exe para abrir ou recuperar o shell, a área de trabalho e a barra de tarefas.','Use quando o Explorer estiver fechado ou indisponível.'],
opcao27:['Limpar temporários e lixeira','Apaga temporários, itens recentes e downloads do Windows Update; limpa DNS e esvazia a Lixeira.','A exclusão da Lixeira é permanente. Feche programas e revise seus arquivos antes.'],
opcao28:['Reparar imagem do Windows','Executa DISM RestoreHealth e SFC para verificar e tentar reparar componentes e arquivos protegidos.','Pode levar bastante tempo e precisar de internet ou mídia do Windows.'],
opcao29:['Desativar Power Throttling','Define PowerThrottlingOff para impedir a limitação de energia controlada por essa política do Windows.','Pode aumentar consumo e temperatura, especialmente em notebooks.'],
opcao30:['Aplicar política de energia (EcoQoS)','Aplica a mesma chave PowerThrottlingOff da opção Power Throttling. Não modifica uma política exclusiva de EcoQoS.','Não é necessário executar junto da opção Power Throttling.'],
opcao31:['Desativar suspensão USB','Desativa a suspensão seletiva USB no plano atual, tanto na tomada quanto na bateria.','Pode aumentar o consumo de energia de periféricos.'],
opcao32:['Desativar economia dos adaptadores','Solicita ao Windows que desative o gerenciamento de energia dos adaptadores físicos de rede.','A disponibilidade depende do driver e pode aumentar o consumo.'],
opcao33:['Desativar economia PCIe','Desativa gerenciamento de energia do link PCI Express no plano atual, na tomada e na bateria.','Pode aumentar consumo e temperatura.'],
opcao34:['Reiniciar computador','Agenda uma reinicialização do Windows em 30 segundos.','Salve seus arquivos. Esta ação só pode ser executada separadamente.'],
autorun:['Gerenciar inicialização','Abre o Microsoft Autoruns para inspecionar entradas que iniciam com o Windows.','A escolha do que desativar é feita na janela do utilitário.'],
fixbluetooth:['Reativar Bluetooth','Configura o serviço de suporte Bluetooth para início automático e tenta iniciá-lo.','Não instala drivers e não corrige defeitos físicos.'],
fixreativarwifi:['Reativar Wi-Fi e serviços de rede','Reativa WLAN, DHCP e serviços de conexão, e redefine Winsock e a pilha IP.','A conexão pode cair; uma reinicialização pode ser necessária.'],
fixaudio:['Reiniciar serviços de áudio','Configura e reinicia os serviços de áudio e de endpoints do Windows.','A reprodução e a gravação serão interrompidas durante o reparo.'],
fixlojadowindows:['Registrar apps da Microsoft Store','Reativa o serviço BITS e tenta registrar manifestos dos aplicativos instalados de todos os usuários.','Não é limitado à Store e não baixa pacotes removidos.'],
fixnotebookmodoaviao:['Reativar gerenciamento de rádio','Reativa o serviço de gerenciamento de rádio e o serviço WLAN para tentar recuperar controles de conexão.','Pode exigir reconectar à rede. Não altera interruptores físicos do dispositivo.'],
fixpcnaodesliga:['Habilitar hibernação e desligar','Ativa a hibernação e agenda o desligamento em 30 segundos.','Não é um diagnóstico de falhas de desligamento. Salve arquivos e execute separadamente.'],
fixarquivoscorrompidos:['Verificar arquivos corrompidos','Executa SFC e depois DISM RestoreHealth para tentar reparar arquivos protegidos e a imagem do Windows.','Pode demorar e precisar de rede ou mídia de instalação.'],
fixderedeeinternet:['Redefinir conexão de rede','Limpa DNS, renova o IP e redefine a pilha TCP/IP e o catálogo Winsock.','Interrompe conexões e pode exigir reconfiguração de rede ou reinicialização.'],
fixerrodedisco:['Verificar disco do sistema','Executa CHKDSK com correção e busca de setores defeituosos na unidade do Windows.','O disco pode estar em uso e exigir agendamento interativo, indisponível neste executor. Revise o registro.'],
fixmenuiniciarebarradetarefas:['Reiniciar área de trabalho','Encerra e inicia o Windows Explorer para tentar recuperar o menu, a barra de tarefas e a área de trabalho.','Janelas do Explorer serão fechadas.'],
fixservicosxbox:['Reconfigurar serviços Xbox','Define início automático para autenticação Xbox Live, salvamento de jogos e rede Xbox.','Não reinstala aplicativos Xbox removidos.'],
fixwindowsupdate:['Redefinir cache de atualizações','Interrompe serviços de atualização, renomeia os caches SoftwareDistribution e catroot2 e reinicia os serviços.','Pode falhar se as pastas de backup já existirem. As atualizações serão interrompidas.'],
fixxboxapp:['Registrar aplicativo Xbox','Interrompe serviços Xbox, tenta registrar os pacotes instalados e reinicia os serviços.','Não baixa pacotes ausentes. Pode afetar sessões de jogos em andamento.'],
fixcamera:['Reparar câmera e permissões','Fecha apps de chamadas, libera acesso à câmera, reinicia dispositivos e serviços e executa DISM/SFC e atualização de componentes.','Encerra Teams, Zoom, Discord e OBS. Amplia permissões da câmera e pode demorar.'],
fixmicrofone:['Reparar microfone e permissões','Fecha apps de chamadas, libera permissões do microfone, reinicia dispositivos de áudio e executa DISM/SFC.','Interrompe chamadas e gravações e amplia permissões de acesso ao microfone.']
};
export function enrich(actions){
 for(const a of actions){
  const script=fs.readFileSync(a.file,'utf8');
  if(a.category==='Jogos'){
   const exes=[...new Set([...script.matchAll(/\\([^\\"\r\n]+\.exe)(?:\\|\")/gi)].map(m=>m[1]))];
   a.description='Define prioridade alta de CPU no Registro para '+(exes.join(', ')||'os executáveis deste jogo')+'. Não altera os arquivos do jogo.';
   a.warning='O efeito depende do jogo e do sistema. Pode afetar outras tarefas; não garante aumento de FPS.';
   if(a.id==='priorizar_re9'){a.title='Priorizar Rust (entrada duplicada)';a.warning='Esta entrada altera RustClient.exe e equivale à ação Priorizar Rust.';}
   if(a.id==='priorizar_marvel')a.title='Priorizar Marvel Rivals (entrada duplicada)';
  }else if(a.category==='Aplicativos'){
   const packages=[...new Set([...script.matchAll(/Get-AppxPackage\s+([^ |"\r\n]+)/gi)].map(m=>m[1]))];
   a.description='Tenta remover do usuário os pacotes '+packages.join(', ')+'.'+(/reg (?:add|delete)/i.test(script)?' Também aplica políticas e ajustes no Registro.':'');
   a.warning='Pode remover funções em uso. Reinstalar pode exigir a Microsoft Store; não há reversão automática.';
   if(a.id==='debloat_opcao22'){a.title='Reduzir anúncios e sugestões';a.description='Desativa sugestões e conteúdo recomendado no Windows, além do rastreamento e exibição de programas e documentos recentes.';a.warning='Altera a apresentação do menu Iniciar e do Explorer; não remove aplicativos.';}
  }else if(['amd','intel','nvidia'].includes(a.id)){
   a.description='Aplica o perfil '+a.id.toUpperCase()+': desativa MPO e capturas, ajusta tela cheia e prioridades multimídia.'+(a.id==='nvidia'?' Também desativa o contêiner de telemetria, se existir.':'');
   a.warning='Use apenas um perfil de hardware por vez. Pode afetar captura e compatibilidade gráfica.';
  }else if(a.id==='reverteramdintelnvidia'){
   a.description='Remove os ajustes de MPO e tela cheia, reativa opções Game Bar e restaura valores fixos do agendamento multimídia.';
   a.warning='Restaura padrões definidos pelo script; não recupera suas configurações a partir de um backup.';
  }else{const d=detail[a.id];if(!d)throw Error('Description missing: '+a.id);[a.title,a.description,a.warning]=d;}
  a.risk=/Remove-Appx|Clear-RecycleBin|Spooler|hypervisorlaunchtype|shutdown |ConsentStore|AppCompat/i.test(script)?'Alto impacto':/start ""|powercfg.cpl/.test(script)?'Ferramenta':'Ajuste';
  if(a.id==='restore')a.risk='Recuperação';
  a.standalone=['opcao25','opcao34','fixpcnaodesliga','fixerrodedisco'].includes(a.id);
  a.restart=/hypervisorlaunchtype|netsh .*reset|shutdown \/r/.test(script);
  a.conflicts=[];
 }
 const pair=(left,right)=>{for(const l of left)for(const r of right){if(l===r)continue;const a=actions.find(x=>x.id===l),b=actions.find(x=>x.id===r);if(a&&b){a.conflicts.push(r);b.conflicts.push(l);}}};
 pair(['amd','intel','nvidia','reverteramdintelnvidia'],['amd','intel','nvidia','reverteramdintelnvidia']);
 pair(['desativarmemoria'],['ativarmemoria']);pair(['desativarsuper'],['ativarsuper']);pair(['otimizarforeground','opcao15'],['voltarforeground']);
 pair(['opcao29'],['opcao30']);pair(['opcao25'],['opcao26']);pair(['opcao9'],['fixpcnaodesliga']);pair(['opcao8'],['fixwindowsupdate']);
 pair(['reverterxbox','fixservicosxbox','fixxboxapp'],['desativarxbox','debloat_opcao7','debloat_opcao2']);
 pair(['reverteramdintelnvidia'],['opcao5','opcao15','opcao20','otimizarforeground','desativarxbox']);
 pair(['reverterdebloater','fixlojadowindows'],actions.filter(a=>/Remove-AppxPackage/i.test(fs.readFileSync(a.file,'utf8'))).map(a=>a.id));
 for(const a of actions)a.conflicts=[...new Set(a.conflicts)];
 return actions;
}
