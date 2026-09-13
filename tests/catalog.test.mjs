import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import crypto from 'node:crypto';
const c=JSON.parse(fs.readFileSync('catalog.json'));
test('every published file matches trusted size and hash',()=>{for(const [p,f] of Object.entries(c.files)){const b=fs.readFileSync(p);assert.equal(b.length,f.size,p);assert.equal(crypto.createHash('sha256').update(b).digest('hex'),f.sha256,p);}});
test('all actions and dependencies exist and actions contain no menu or dangling calls',()=>{assert.equal(new Set(c.actions.map(a=>a.id)).size,c.actions.length);for(const a of c.actions){assert.ok(c.files[a.file]);for(const d of a.dependencies)assert.ok(c.files[d],d);let b=fs.readFileSync(a.file,'utf8');assert.doesNotMatch(b,/^\s*(?:set \/p|goto :?\w+)/mi,a.id);const labels=[...b.matchAll(/^:([a-z0-9_]+)/gmi)].map(m=>m[1]);for(const m of b.matchAll(/call :([a-z0-9_]+)/gi))assert.ok(labels.includes(m[1]),a.id);assert.doesNotMatch(b,/igust|windows boost/i);}});
test('known broken original menu options resolve independently',()=>{for(const id of ['restore','opcao1','opcao34','priorizar_rust','priorizar_ets1','priorizar_ets2','priorizar_tlou1','fixcamera','fixmicrofone'])assert.ok(c.actions.some(a=>a.id===id),id);assert.match(fs.readFileSync('scripts/actions/restore.bat','utf8'),/ErrorAction Stop/);});
test('every action has specific descriptions and symmetric conflict metadata',()=>{for(const a of c.actions){assert.ok(a.description.length>40,a.id);assert.ok(a.warning.length>20,a.id);assert.ok(a.risk,a.id);for(const id of a.conflicts){assert.ok(c.actions.find(b=>b.id===id)?.conflicts.includes(a.id),a.id);}}assert.match(c.actions.find(a=>a.id==='opcao27').description,/Lixeira/);assert.equal(c.actions.find(a=>a.id==='opcao34').standalone,true);});
