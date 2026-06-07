import{r as i,i as w,j as s,f as I}from"./index-0uk_KvXQ.js";import{r as j}from"./overlay-url-yJYaczpp.js";import{m as C,n as v,b as E,G as p,a as O}from"./gift-goal-ByACYXf6.js";const S="http://127.0.0.1:35942";function G(o){const f=new Set,g=m=>{const d=String(m||"").trim().replace(/\/$/,"");d&&f.add(d)};return g(o),g(S),g("https://liveplay-backend.onrender.com/overlay-bridge"),Array.from(f)}function N(o){return new Intl.NumberFormat("pt-BR",{notation:Number(o)>=1e4?"compact":"standard"}).format(Math.max(0,Number(o||0)))}function F(o){switch(o){case"top-left":return{position:"fixed",top:24,left:24,transformOrigin:"top left"};case"top-right":return{position:"fixed",top:24,right:24,transformOrigin:"top right"};case"bottom-left":return{position:"fixed",bottom:24,left:24,transformOrigin:"bottom left"};case"bottom-right":return{position:"fixed",bottom:24,right:24,transformOrigin:"bottom right"};default:return{position:"fixed",left:"50%",top:"50%",transform:"translate(-50%, -50%)",transformOrigin:"center"}}}function A(){const[o,f]=i.useState(()=>C()),[g,m]=i.useState(0),d=i.useRef(""),k=i.useMemo(()=>new URLSearchParams(window.location.search),[]),x=j(k,"/overlay-bridge",S),u=i.useCallback(t=>{if((t==null?void 0:t.type)!=="gift-goal-config"||!t.config)return;const e=v(t.config),r=JSON.stringify(e);r!==d.current&&(d.current=r,f(e))},[]),l=i.useCallback(t=>{const e=(t==null?void 0:t.payload)??t;if((e==null?void 0:e.type)==="gift-goal-config"){u(e);return}if((e==null?void 0:e.type)==="gift-goal-event"&&e.event){f(r=>{const a=E(r,e.event);try{window.localStorage.setItem(p,JSON.stringify(a))}catch{}return w(p,a).catch(()=>{}),a});return}(e==null?void 0:e.type)==="gift-goal-reset"&&f(r=>{const a=v({...r,goals:r.goals.map(n=>({...n,current:0}))});try{window.localStorage.setItem(p,JSON.stringify(a))}catch{}return w(p,a).catch(()=>{}),a})},[u]);i.useEffect(()=>{let t=!0;return(async()=>{const r=await I(p,null).catch(()=>null);t&&r&&u({type:"gift-goal-config",config:r});try{const a=window.localStorage.getItem(p);t&&a&&u({type:"gift-goal-config",config:JSON.parse(a)})}catch{}})(),()=>{t=!1}},[u]),i.useEffect(()=>{let t=!1;const e=async()=>{if(!t)for(const a of G(x))try{const n=await fetch(`${a}/snapshot?type=gift-goal-config&_=${Date.now()}`,{cache:"no-store"});l(await n.json())}catch{}};e();const r=window.setInterval(e,5e3);return()=>{t=!0,window.clearInterval(r)}},[l,x]),i.useEffect(()=>{const t=[];for(const e of G(x))for(const r of["gift-goal-config","gift-goal-event","gift-goal-reset"])try{const a=new EventSource(`${e}/events?type=${r}`);a.onmessage=n=>{try{l(JSON.parse(n.data))}catch{}},t.push(a)}catch{}return()=>t.forEach(e=>e.close())},[l,x]),i.useEffect(()=>{var r,a,n;let t=null;try{t=new BroadcastChannel(O),t.onmessage=h=>l(h.data)}catch{}const e=(n=(a=(r=window.liveplay)==null?void 0:r.app)==null?void 0:a.onOverlaySync)==null?void 0:n.call(a,h=>l(h));return()=>{t==null||t.close(),typeof e=="function"&&e()}},[l]);const c=i.useMemo(()=>{const t=o.goals.filter(a=>a.enabled&&a.giftName),e=o.skipCompleted?t.filter(a=>Math.max(0,Number(a.current||0))<Math.max(1,Number(a.target||1))):t,r=e.length>0?e:t;return(o.orderMode||"closest")==="closest"?[...r].sort((a,n)=>{const h=Math.max(0,Number(a.current||0))/Math.max(1,Number(a.target||1));return Math.max(0,Number(n.current||0))/Math.max(1,Number(n.target||1))-h}):r},[o.goals,o.skipCompleted,o.orderMode]),b=Math.max(0,Math.min(1,Number(o.opacity||94)/100)),y=Math.max(2e3,Math.min(6e4,Number(o.rotateSeconds||5)*1e3));i.useEffect(()=>{if(c.length<=1){m(0);return}m(e=>e%c.length);const t=window.setInterval(()=>m(e=>(e+1)%c.length),y);return()=>window.clearInterval(t)},[c.length,y]);const M=c.length>0?[c[g%c.length]]:[];return s.jsxs("div",{className:"gift-goal-overlay",style:{opacity:o.enabled?1:0,pointerEvents:"none"},children:[s.jsx("style",{children:`
        html, body, #root { width: 100%; height: 100%; margin: 0; background: transparent !important; overflow: hidden; }
        .gift-goal-overlay { width: 100%; height: 100%; font-family: Inter, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; color: white; perspective: 1200px; }
        .gift-goal-stack {
          display: flex;
          align-items: center;
          justify-content: center;
          width: min(420px, calc(100vw - 40px));
          transform: scale(var(--goal-scale));
        }
        .gift-goal-card {
          --accent: #ffd42a;
          --title-color: #ffffff;
          --count-color: #ffffff;
          --target-color: #ffd42a;
          display: flex;
          flex-direction: column;
          align-items: center;
          justify-content: flex-start;
          width: min(380px, calc(100vw - 40px));
          text-align: center;
          background: transparent;
          padding: 0;
          transform-style: preserve-3d;
          filter: drop-shadow(0 10px 22px rgba(0,0,0,.34));
        }
        .gift-goal-effect-box {
          display: flex;
          flex-direction: column;
          align-items: center;
          width: 100%;
          transform-origin: center;
          will-change: transform, filter;
        }
        .gift-goal-label {
          font-size: 36px;
          line-height: 1.02;
          font-weight: 1000;
          letter-spacing: .01em;
          color: var(--title-color);
          text-transform: none;
          text-shadow: 0 3px 14px rgba(0,0,0,.46);
          margin-bottom: 12px;
        }
        .gift-goal-image { width: 190px; height: 190px; display: grid; place-items: center; margin-bottom: 14px; background: transparent; }
        .gift-goal-image img { width: 180px; height: 180px; object-fit: contain; filter: drop-shadow(0 14px 22px rgba(0,0,0,.30)); }
        .gift-goal-progress {
          margin-top: 0;
          font-size: 48px;
          line-height: 1;
          font-weight: 1000;
          color: var(--count-color);
          text-shadow: 0 3px 14px rgba(0,0,0,.42);
        }
        .gift-goal-progress .gift-goal-target { font-size: 30px; font-weight: 950; color: var(--target-color); margin-left: 8px; }
        .gift-goal-skin-glass .gift-goal-card { padding: 18px 20px 20px; border-radius: 30px; background: rgba(8,13,28,calc(var(--goal-opacity) * .28)); border: 1px solid rgba(255,255,255,.10); backdrop-filter: blur(8px); }
        .gift-goal-skin-clean .gift-goal-card { filter: drop-shadow(0 7px 14px rgba(0,0,0,.26)); }
        .gift-goal-effect-pulse .gift-goal-effect-box { animation: giftGoalPulse 1.75s ease-in-out infinite; }
        .gift-goal-effect-float .gift-goal-effect-box { animation: giftGoalFloat 2.8s ease-in-out infinite; }
        .gift-goal-effect-neon .gift-goal-card { filter: drop-shadow(0 10px 22px rgba(0,0,0,.34)) drop-shadow(0 0 22px color-mix(in srgb, var(--accent) 72%, transparent)); }
        .gift-goal-effect-neon .gift-goal-label,
        .gift-goal-effect-neon .gift-goal-progress,
        .gift-goal-effect-neon .gift-goal-percent { text-shadow: 0 3px 14px rgba(0,0,0,.46), 0 0 22px color-mix(in srgb, var(--accent) 58%, transparent); }
        .gift-goal-effect-neon .gift-goal-image img { filter: drop-shadow(0 14px 22px rgba(0,0,0,.30)) drop-shadow(0 0 22px color-mix(in srgb, var(--accent) 58%, transparent)); }
        .gift-goal-effect-pop .gift-goal-image img { animation: giftGoalImagePop 1.55s ease-in-out infinite; transform-origin: center; }
        .gift-goal-transition-loop .gift-goal-card { animation: giftGoalLoop ${Math.max(1.8,Math.min(8,Number(o.rotateSeconds||5)))}s ease-in-out infinite; }
        .gift-goal-transition-fade .gift-goal-card { animation: giftGoalFade .58s ease both; }
        .gift-goal-transition-slide .gift-goal-card { animation: giftGoalSlide .58s cubic-bezier(.2,.8,.2,1) both; }
        .gift-goal-transition-flip .gift-goal-card { animation: giftGoalFlip .68s cubic-bezier(.2,.8,.2,1) both; }
        @keyframes giftGoalPop { from { opacity: 0; transform: translateY(12px) scale(.96); } to { opacity: 1; transform: translateY(0) scale(1); } }
        @keyframes giftGoalPulse { 0%, 100% { transform: scale(1); } 50% { transform: scale(1.045); } }
        @keyframes giftGoalFloat { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-8px); } }
        @keyframes giftGoalImagePop { 0%, 100% { transform: scale(1); } 45% { transform: scale(1.08); } }
        @keyframes giftGoalLoop { 0%, 100% { transform: scale(1); filter: drop-shadow(0 10px 22px rgba(0,0,0,.34)); } 50% { transform: scale(1.035); filter: drop-shadow(0 10px 22px rgba(0,0,0,.34)) drop-shadow(0 0 20px color-mix(in srgb, var(--accent) 70%, transparent)); } }
        @keyframes giftGoalFade { from { opacity: 0; transform: scale(.94); } to { opacity: 1; transform: scale(1); } }
        @keyframes giftGoalSlide { from { opacity: 0; transform: translateX(42px) scale(.96); } to { opacity: 1; transform: translateX(0) scale(1); } }
        @keyframes giftGoalFlip { from { opacity: 0; transform: rotateY(-74deg) scale(.92); } to { opacity: 1; transform: rotateY(0) scale(1); } }
      `}),s.jsx("div",{className:`gift-goal-stack gift-goal-skin-${o.skin} gift-goal-effect-${o.effect||"none"} gift-goal-mode-carousel gift-goal-transition-${o.transition||"fade"}`,style:{...F(o.position),"--goal-scale":String(o.scale),"--goal-opacity":String(b)},children:M.map(t=>{const e=Math.max(1,Number(t.target||1)),r=Math.max(0,Number(t.current||0));return s.jsx("div",{className:"gift-goal-card",style:{"--accent":t.accent||"#ffd42a","--title-color":o.titleColor||"#ffffff","--count-color":o.countColor||"#ffffff","--target-color":o.targetColor||"#ffd42a",opacity:b},children:s.jsxs("div",{className:"gift-goal-effect-box",children:[s.jsx("div",{className:"gift-goal-label",children:o.showHeader&&o.title||"Meta"}),s.jsx("div",{className:"gift-goal-image",children:s.jsx("img",{src:t.giftImageUrl||"./events/gift.svg",alt:t.giftName})}),s.jsxs("div",{className:"gift-goal-progress",children:[N(r),s.jsxs("span",{className:"gift-goal-target",children:["/ ",N(e)]})]})]})},`${t.id}-${g}`)})})]})}export{A as GiftGoalOverlayPage};
