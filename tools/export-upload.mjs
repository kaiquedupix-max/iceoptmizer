import fs from 'node:fs';
const walk=d=>fs.readdirSync(d,{withFileTypes:true}).flatMap(e=>e.isDirectory()?walk(d+'/'+e.name):[d+'/'+e.name]);
const files=['.gitignore','.gitattributes','README.md','THIRD-PARTY-NOTICES.md','build.ps1','catalog.json',...walk('src'),...walk('scripts'),...walk('tools'),...walk('tests')];
const isText=p=>/\.(?:md|ps1|json|mjs|cs|manifest|bat|ini|txt|config)$|^\./.test(p)&&fs.readFileSync(p).equals(Buffer.from(fs.readFileSync(p,'utf8'),'utf8'));
if(process.argv[2]==='chunk'){
 const data=process.argv[3]==='text'?JSON.stringify(files.filter(isText).map(p=>({path:p,mode:'100644',type:'blob',content:fs.readFileSync(p,'utf8')}))):fs.readFileSync(process.argv[3]).toString('base64');
 const offset=Number(process.argv[4]||0);console.log(JSON.stringify({length:data.length,data:data.slice(offset,offset+90000)}));process.exit(0);
}
if(process.argv[2]==='list')console.log(JSON.stringify(files));
else if(process.argv[2]==='text')console.log(JSON.stringify(files.filter(p=>/\.(?:md|ps1|json|mjs|cs|manifest|bat|ini|txt|config)$|^\./.test(p)).map(p=>({path:p,mode:'100644',type:'blob',content:fs.readFileSync(p,'utf8')}))));
else if(process.argv[2]==='binary')console.log(JSON.stringify(files.filter(p=>!isText(p))));
else if(process.argv[2]==='file')console.log(fs.readFileSync(process.argv[3]).toString('base64'));
