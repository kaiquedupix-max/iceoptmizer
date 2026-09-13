import fs from 'node:fs';
import path from 'node:path';
import crypto from 'node:crypto';
import {enrich} from './descriptions.mjs';
const read=p=>fs.readFileSync(p,'utf8').replace(/\r/g,'');
const branding=s=>s.replace(/iGust(?: Windows Boost)?|Windows Boost/gi,'ice optimizer').replace(/^set "line([1-7])=.*$/gmi,(_,i)=>'set "line'+i+'='+(i==='1'?'ice optimizer - por Maciota':'')+'"').replace(/^set "lines\[(\d+)\]=.*$/gmi,(_,i)=>'set "lines['+i+']='+(i==='0'?'ice optimizer - por Maciota':'')+'"');
for(const file of ['scripts/engine.bat','scripts/debloater.bat']) fs.writeFileSync(file,branding(read(file)).replace(/\n/g,'\r\n'));
const engine=read('scripts/engine.bat'), debloat=read('scripts/debloater.bat');
function blocks(s){const m=[...s.matchAll(/^:([a-z0-9_]+)\s*$/gmi)];return m.map((x,i)=>({id:x[1].toLowerCase(),body:s.slice(x.index+x[0].length,m[i+1]?.index??s.length)}));}
const all=blocks(engine);
for(const b of all)if(b.id==='opcao1'&&b.body.includes('Checkpoint-Computer'))b.id='restore';
const windows=['','Otimizar energia','Desativar efeitos visuais','Desativar apps em segundo plano','Desativar serviços (inclui impressão e biometria)','Otimizar Game Bar','Xbox','Desativar relatórios de erro','Desativar telemetria','Desativar hibernação','Compressão de memória','Desativar indexação','Otimizar menu Iniciar','Desativar Cortana','Prefetch e SysMain','Prioridade de CPU e GPU','Prioridade de primeiro plano','Desativar isolamento de núcleo','Debloater','Ativar Modo de Jogo','Desativar otimizações de tela cheia','Desativar aceleração do mouse','Abrir ISLC / resolução de timer','Desativar VBS e HVCI','Desativar compatibilidade forçada','Fechar Explorer','Iniciar Explorer','Limpar cache do Windows','Verificar arquivos do Windows','Desativar Power Throttling','Desativar EcoQoS','Desativar suspensão seletiva USB','Desativar economia da rede','Desativar economia PCIe','Reiniciar computador'];
const names={restore:'Criar ponto de restauração e backup',ping:'Renovar conexão e abrir DNS Jumper',limparram:'Liberar memória RAM',amd:'Ajustes AMD',intel:'Ajustes Intel',nvidia:'Ajustes NVIDIA',reverteramdintelnvidia:'Restaurar padrões de GPU',autorun:'Gerenciar inicialização / Autoruns',desativarxbox:'Desativar serviços Xbox',reverterxbox:'Reativar serviços Xbox',desativarmemoria:'Desativar compressão de memória',ativarmemoria:'Ativar compressão de memória',desativarsuper:'Desativar Prefetch e SysMain',ativarsuper:'Reativar Prefetch e SysMain',otimizarforeground:'Priorizar aplicativo em primeiro plano',voltarforeground:'Restaurar prioridade de primeiro plano',debloater:'Remover aplicativos em lote',reverterdebloater:'Registrar novamente aplicativos',fixbluetooth:'Reparar Bluetooth',fixreativarwifi:'Reativar Wi-Fi',fixaudio:'Reparar áudio',fixlojadowindows:'Reparar Microsoft Store',fixnotebookmodoaviao:'Reparar modo avião',fixpcnaodesliga:'Desligar computador',fixarquivoscorrompidos:'Reparar arquivos corrompidos',fixderedeeinternet:'Redefinir rede e internet',fixerrodedisco:'Verificar disco do sistema',fixmenuiniciarebarradetarefas:'Reiniciar Explorer e barra de tarefas',fixservicosxbox:'Reparar serviços Xbox',fixwindowsupdate:'Reparar Windows Update',fixxboxapp:'Reparar aplicativo Xbox',fixcamera:'Reparar câmera',fixmicrofone:'Reparar microfone'};
const menus=new Set(['menu','sair','amdintelnvidia','menuwindows','prioridadegames','fix','step','escolherdebloater','opcao6','opcao10','opcao14','opcao16','opcao18']);
const actions=[]; fs.mkdirSync('scripts/actions',{recursive:true});
function add(b,category,title){
 let body=b.body.replace(/^\s*(?:pause(?: .*)?|cls|goto\s+:?\w+|start "" "%~f0"|exit(?: \/b(?: \d+)?)?)\s*$/gmi,'').replace(/reg add (.*) \/v (GPU Priority|Scheduling Category|SFIO Priority) \/t/g,'reg add $1 /v "$2" /t');
 body=body.replace(/shutdown \/f \/t 0/g,'shutdown /s /t 30').replace(/shutdown \/r \/t 0/g,'shutdown /r /t 30');
 body=body.replace(/\bC: \/f/gi,'%SystemDrive% /f');
 // Output is captured by the GUI; avoid falsely declaring successful multi-command jobs.
 body=body.replace(/^echo .*sucesso.*$/gmi,'echo Etapa encerrada. Verifique as mensagens acima.');
 if(/set \/p|^\s*if .*goto /mi.test(body))throw Error('Interactive block: '+b.id);
 const prefix='@echo off\nchcp 65001 >nul\nsetlocal EnableExtensions EnableDelayedExpansion\nrem ice optimizer - Interface e integracao por Maciota\nset "LOG=%~dp0details.log"\n';
 const suffix='\nexit /b %errorlevel%\n'+(body.includes('call :step')?'\n:step\necho [*] %~1\nexit /b 0\n':'');
 const file='scripts/actions/'+b.id+'.bat';fs.writeFileSync(file,(prefix+body+suffix).replace(/\n/g,'\r\n'));
 let warning='Altera configurações do Windows. Pode exigir reinicialização. Consulte o registro ao terminar.';
 if(category==='Jogos')warning='Ajusta a prioridade dos executáveis deste jogo no Registro. Não altera arquivos do jogo nem garante aumento de FPS.';
 if(/Remove-AppxPackage|Remove-AppxProvisionedPackage/i.test(body))warning='Remove aplicativos; pode remover Store, Xbox ou programas usados por você. A reinstalação pode exigir a Microsoft Store ou mídia do Windows.';
 if(/Spooler/.test(body))warning+=' Esta ação também desativa impressão e biometria.';
 if(/EnableVirtualizationBasedSecurity|HypervisorEnforcedCodeIntegrity|hypervisorlaunchtype/i.test(body))warning='Reduz proteções de isolamento e virtualização do Windows. Pode afetar segurança e compatibilidade. Reinicialização necessária.';
 if(/shutdown /i.test(body))warning='Solicita desligamento ou reinicialização em 30 segundos. Salve os seus arquivos antes de executar.';
 if(/ipconfig \/release|netsh .*reset/i.test(body))warning='A conexão será interrompida temporariamente. Pode exigir reconexão ou reinicialização.';
 if(b.id==='restore')warning='Cria ponto de restauração e exporta chaves do Registro. A Proteção do Sistema precisa estar disponível. Confira o resultado no registro.';
 const deps=[];
 if(body.includes('DnsJumper.exe'))deps.push('scripts/DnsJumper.exe','scripts/DnsJumper.ini');
 if(body.includes('EmptyStandbyList.exe'))deps.push('scripts/EmptyStandbyList.exe');
 if(body.includes('Autoruns.exe'))deps.push('scripts/Autoruns.exe');
 if(body.includes('ISLC v1.0.4.6'))deps.push(...walk('scripts/ISLC v1.0.4.6'));
 actions.push({id:b.id,title,category,warning,file,dependencies:deps});
}
function walk(dir){return fs.readdirSync(dir,{withFileTypes:true}).flatMap(e=>e.isDirectory()?walk(dir+'/'+e.name):[dir+'/'+e.name]);}
for(const b of all){if(menus.has(b.id))continue;const game=b.id.startsWith('priorizar_');let title=names[b.id]||windows[Number(b.id.replace('opcao',''))];if(game)title='Priorizar '+b.id.replace('priorizar_','').replaceAll('_',' ').toUpperCase();if(!title)throw Error('Missing name '+b.id);add(b,game?'Jogos':b.id.startsWith('fix')?'Reparos':['amd','intel','nvidia','reverteramdintelnvidia'].includes(b.id)?'Hardware':b.id==='restore'?'Recuperação':'Windows',title);}
for(const b of blocks(debloat)){if(!/^opcao\d+$/.test(b.id)||b.id==='opcao1')continue;const label=(b.body.match(/echo (?:Removendo|Desativando) (.+)/i)?.[1]||'Todos os aplicativos').replace(/\.{2,}/g,'').trim();add({...b,id:'debloat_'+b.id},'Aplicativos','Remover: '+label);}
// Fix restore deterministically: original batch had duplicate labels and ignored PowerShell errors.
fs.writeFileSync('scripts/actions/restore.bat','@echo off\r\nchcp 65001 >nul\r\npowershell.exe -NoProfile -Command "try { Checkpoint-Computer -Description \'ice optimizer - Maciota\' -RestorePointType MODIFY_SETTINGS -ErrorAction Stop } catch { Write-Error $_; exit 1 }"\r\nif errorlevel 1 exit /b 1\r\nif not exist "%~dp0Backup" mkdir "%~dp0Backup"\r\nreg export "HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer" "%~dp0Backup\\Explorer.reg" /y\r\nif errorlevel 1 exit /b 1\r\nreg export "HKCU\\Control Panel\\Desktop" "%~dp0Backup\\Desktop.reg" /y\r\nexit /b %errorlevel%\r\n');
const files=Object.fromEntries(walk('scripts').map(p=>[p,{sha256:crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex'),size:fs.statSync(p).size}]));
fs.writeFileSync('catalog.json',JSON.stringify({name:'ice optimizer',author:'Maciota',version:'2.1.0',repository:'kaiquedupix-max/iceoptmizer',ref:'main',licenseApi:'https://iceoptmizer-production.up.railway.app',actions:enrich(actions),files},null,2));
console.log(`${actions.length} actions generated; ${Object.keys(files).length} files indexed.`);
