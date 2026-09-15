(()=>{
 const staticMode=new URLSearchParams(location.search).has('static');if(!staticMode)document.body.classList.add('motion-ready');
 const progress=document.createElement('div');progress.className='scroll-progress';progress.innerHTML='<i></i>';document.body.appendChild(progress);
 ['a','b','c'].forEach(name=>{const orb=document.createElement('div');orb.className='aurora '+name;document.body.appendChild(orb)});
 let scrollQueued=false;function updateScroll(){const max=Math.max(1,document.documentElement.scrollHeight-innerHeight),ratio=Math.min(1,scrollY/max);progress.firstElementChild.style.transform=`scaleX(${ratio})`;document.querySelectorAll('.aurora').forEach((orb,index)=>orb.style.transform=`translate3d(0,${scrollY*(.035+index*.018)}px,0)`);scrollQueued=false}addEventListener('scroll',()=>{if(!scrollQueued){scrollQueued=true;requestAnimationFrame(updateScroll)}},{passive:true});updateScroll();
 const snow=document.querySelector('.snow');
 if(snow){for(let i=0;i<68;i++){const f=document.createElement('span');f.className='flake';f.textContent=i%5?'•':'❄';f.style.left=Math.random()*100+'vw';f.style.fontSize=6+Math.random()*15+'px';f.style.animationDuration=9+Math.random()*15+'s';f.style.animationDelay=-Math.random()*22+'s';f.style.setProperty('--drift',(-70+Math.random()*140)+'px');snow.appendChild(f)}}

 const reveals=[...document.querySelectorAll('.reveal')];
 reveals.forEach((el,index)=>{el.style.setProperty('--reveal-delay',(index%4)*70+'ms');if(index%3===1)el.classList.add('from-left');if(index%3===2)el.classList.add('from-right')});
 const observer=new IntersectionObserver(entries=>entries.forEach(entry=>{if(entry.isIntersecting){entry.target.classList.add('visible');observer.unobserve(entry.target)}}),{threshold:.12,rootMargin:'0px 0px -25px'});
 reveals.forEach(el=>observer.observe(el));
 reveals.filter(el=>el.getBoundingClientRect().top<innerHeight*.98).forEach(el=>el.classList.add('visible'));

 const product=document.querySelector('.product');
 if(product)product.addEventListener('pointermove',event=>{if(innerWidth<900)return;const box=product.getBoundingClientRect(),x=(event.clientX-box.left)/box.width-.5,y=(event.clientY-box.top)/box.height-.5;product.style.transform=`perspective(1200px) rotateY(${x*5}deg) rotateX(${-y*4}deg) translateY(-5px) scale(1.012)`});
 if(product)product.addEventListener('pointerleave',()=>product.style.transform='');
 document.querySelectorAll('.feature,.plan').forEach(card=>card.addEventListener('pointermove',event=>{const box=card.getBoundingClientRect();card.style.setProperty('--mx',event.clientX-box.left+'px');card.style.setProperty('--my',event.clientY-box.top+'px')}));

 let audioContext,master,chimeTimer,soundOn=false;
 const soundButton=document.querySelector('#sound-toggle');
 function setSoundLabel(){if(!soundButton)return;soundButton.setAttribute('aria-pressed',String(soundOn));soundButton.textContent=soundOn?'♫ Som ligado':'♫ Som glacial'}
 function chime(){if(!soundOn||!audioContext)return;const now=audioContext.currentTime,osc=audioContext.createOscillator(),gain=audioContext.createGain();osc.type='sine';osc.frequency.setValueAtTime([523.25,659.25,783.99][Math.floor(Math.random()*3)],now);gain.gain.setValueAtTime(0,now);gain.gain.linearRampToValueAtTime(.018,now+.08);gain.gain.exponentialRampToValueAtTime(.0001,now+2.8);osc.connect(gain).connect(master);osc.start(now);osc.stop(now+3)}
 function startSound(){if(soundOn)return;audioContext=audioContext||new (window.AudioContext||window.webkitAudioContext)();if(!master){master=audioContext.createGain();master.gain.value=.32;master.connect(audioContext.destination);[130.81,196,261.63].forEach((hz,i)=>{const osc=audioContext.createOscillator(),gain=audioContext.createGain();osc.type='sine';osc.frequency.value=hz;gain.gain.value=[.014,.009,.006][i];osc.connect(gain).connect(master);osc.start()})}audioContext.resume();soundOn=true;setSoundLabel();chime();chimeTimer=setInterval(chime,7200)}
 function stopSound(){soundOn=false;if(chimeTimer)clearInterval(chimeTimer);chimeTimer=null;if(audioContext)audioContext.suspend();setSoundLabel()}
 if(soundButton)soundButton.addEventListener('click',event=>{event.stopPropagation();soundOn?stopSound():startSound()});
 document.addEventListener('pointerdown',event=>{if(!soundOn&&event.target!==soundButton)startSound()},{once:true});
 setSoundLabel();

 const msg=document.querySelector('#checkout-message');
 document.querySelectorAll('.buy').forEach(button=>button.addEventListener('click',async()=>{const old=button.textContent;document.querySelectorAll('.buy').forEach(x=>x.disabled=true);button.textContent='Abrindo pagamento seguro...';if(msg)msg.textContent='Você será direcionado ao checkout seguro do Stripe.';try{const response=await fetch('/api/checkout',{method:'POST',headers:{'content-type':'application/json'},body:JSON.stringify({product:button.dataset.product})}),data=await response.json();if(!response.ok)throw Error(data.message||'Não foi possível iniciar o pagamento.');location.href=data.url}catch(error){if(msg)msg.textContent=error.message;document.querySelectorAll('.buy').forEach(x=>x.disabled=false);button.textContent=old}}));
})();
