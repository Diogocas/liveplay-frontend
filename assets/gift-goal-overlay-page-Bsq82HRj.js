import{r as n,s as v,j as l,g as j}from"./index-CzDRWESM.js";import{r as M}from"./overlay-url-IdlNOR_r.js";import{m as z,n as w,a as C,G as m,b as O}from"./gift-goal-DxayARuM.js";const S="http://127.0.0.1:35942";function G(a){const i=new Set,g=p=>{const d=String(p||"").trim().replace(/\/$/,"");d&&i.add(d)};return g(a),g(S),g("https://liveplay-backend.onrender.com/overlay-bridge"),Array.from(i)}function N(a){return new Intl.NumberFormat("pt-BR",{notation:Number(a)>=1e4?"compact":"standard"}).format(Math.max(0,Number(a||0)))}function E(a){const i={position:"fixed",inset:0,width:"100vw",height:"100vh",display:"flex",boxSizing:"border-box",padding:8,overflow:"hidden"};switch(a){case"top-left":return{...i,alignItems:"flex-start",justifyContent:"flex-start",transformOrigin:"top left"};case"top-right":return{...i,alignItems:"flex-start",justifyContent:"flex-end",transformOrigin:"top right"};case"bottom-left":return{...i,alignItems:"flex-end",justifyContent:"flex-start",transformOrigin:"bottom left"};case"bottom-right":return{...i,alignItems:"flex-end",justifyContent:"flex-end",transformOrigin:"bottom right"};default:return{...i,alignItems:"center",justifyContent:"center",transformOrigin:"center"}}}function $(a){switch(a){case"top-left":return"top left";case"top-right":return"top right";case"bottom-left":return"bottom left";case"bottom-right":return"bottom right";default:return"center"}}function L(){const[a,i]=n.useState(()=>z()),[g,p]=n.useState(0),d=n.useRef(""),k=n.useMemo(()=>new URLSearchParams(window.location.search),[]),x=M(k,"/overlay-bridge",S),u=n.useCallback(t=>{if((t==null?void 0:t.type)!=="gift-goal-config"||!t.config)return;const e=w(t.config),r=JSON.stringify(e);r!==d.current&&(d.current=r,i(e))},[]),f=n.useCallback(t=>{const e=(t==null?void 0:t.payload)??t;if((e==null?void 0:e.type)==="gift-goal-config"){u(e);return}if((e==null?void 0:e.type)==="gift-goal-event"&&e.event){i(r=>{const o=C(r,e.event);try{window.localStorage.setItem(m,JSON.stringify(o))}catch{}return v(m,o).catch(()=>{}),o});return}(e==null?void 0:e.type)==="gift-goal-reset"&&i(r=>{const o=w({...r,goals:r.goals.map(s=>({...s,current:0}))});try{window.localStorage.setItem(m,JSON.stringify(o))}catch{}return v(m,o).catch(()=>{}),o})},[u]);n.useEffect(()=>{let t=!0;return(async()=>{const r=await j(m,null).catch(()=>null);t&&r&&u({type:"gift-goal-config",config:r});try{const o=window.localStorage.getItem(m);t&&o&&u({type:"gift-goal-config",config:JSON.parse(o)})}catch{}})(),()=>{t=!1}},[u]),n.useEffect(()=>{let t=!1;const e=async()=>{if(!t)for(const o of G(x))try{const s=await fetch(`${o}/snapshot?type=gift-goal-config&_=${Date.now()}`,{cache:"no-store"});f(await s.json())}catch{}};e();const r=window.setInterval(e,5e3);return()=>{t=!0,window.clearInterval(r)}},[f,x]),n.useEffect(()=>{const t=[];for(const e of G(x))for(const r of["gift-goal-config","gift-goal-event","gift-goal-reset"])try{const o=new EventSource(`${e}/events?type=${r}`);o.onmessage=s=>{try{f(JSON.parse(s.data))}catch{}},t.push(o)}catch{}return()=>t.forEach(e=>e.close())},[f,x]),n.useEffect(()=>{var r,o,s;let t=null;try{t=new BroadcastChannel(O),t.onmessage=h=>f(h.data)}catch{}const e=(s=(o=(r=window.liveplay)==null?void 0:r.app)==null?void 0:o.onOverlaySync)==null?void 0:s.call(o,h=>f(h));return()=>{t==null||t.close(),typeof e=="function"&&e()}},[f]);const c=n.useMemo(()=>{const t=a.goals.filter(o=>o.enabled&&o.giftName),e=a.skipCompleted?t.filter(o=>Math.max(0,Number(o.current||0))<Math.max(1,Number(o.target||1))):t,r=e.length>0?e:t;return(a.orderMode||"closest")==="closest"?[...r].sort((o,s)=>{const h=Math.max(0,Number(o.current||0))/Math.max(1,Number(o.target||1));return Math.max(0,Number(s.current||0))/Math.max(1,Number(s.target||1))-h}):r},[a.goals,a.skipCompleted,a.orderMode]),b=Math.max(0,Math.min(1,Number(a.opacity||94)/100)),y=Math.max(2e3,Math.min(6e4,Number(a.rotateSeconds||5)*1e3));n.useEffect(()=>{if(c.length<=1){p(0);return}p(e=>e%c.length);const t=window.setInterval(()=>p(e=>(e+1)%c.length),y);return()=>window.clearInterval(t)},[c.length,y]);const I=c.length>0?[c[g%c.length]]:[];return l.jsxs("div",{className:"gift-goal-overlay",style:{opacity:a.enabled?1:0,pointerEvents:"none"},children:[l.jsx("style",{children:`
        html, body, #root { width: 100%; height: 100%; margin: 0; background: transparent !important; overflow: hidden; }
        .gift-goal-overlay { width: 100%; height: 100%; font-family: Inter, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; color: white; perspective: 1200px; }
        .gift-goal-stack {
          display: flex;
          align-items: center;
          justify-content: center;
          width: auto;
          max-width: 100vw;
          max-height: 100vh;
        }
        .gift-goal-scale-box {
          display: flex;
          align-items: center;
          justify-content: center;
          transform: scale(var(--goal-scale));
          transform-origin: var(--goal-scale-origin, center);
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
          width: min(420px, calc(100vw - 16px));
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
          font-size: var(--title-size);
          line-height: 1.02;
          font-weight: 1000;
          letter-spacing: .01em;
          color: var(--title-color);
          text-transform: none;
          text-shadow: 0 3px 14px rgba(0,0,0,.46);
          margin-bottom: 12px;
        }
        .gift-goal-image { width: calc(var(--gift-size) + 10px); height: calc(var(--gift-size) + 10px); display: grid; place-items: center; margin-bottom: 14px; background: transparent; }
        .gift-goal-image img { width: var(--gift-size); height: var(--gift-size); object-fit: contain; filter: drop-shadow(0 14px 22px rgba(0,0,0,.30)); }
        .gift-goal-progress {
          margin-top: 0;
          font-size: var(--count-size);
          line-height: 1;
          font-weight: 1000;
          color: var(--count-color);
          text-shadow: 0 3px 14px rgba(0,0,0,.42);
        }
        .gift-goal-progress .gift-goal-target { font-size: calc(var(--count-size) * .62); font-weight: 950; color: var(--target-color); margin-left: 8px; }
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
        .gift-goal-transition-loop .gift-goal-card { animation: giftGoalLoop ${Math.max(1.8,Math.min(8,Number(a.rotateSeconds||5)))}s ease-in-out infinite; }
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
      `}),l.jsx("div",{className:`gift-goal-stack gift-goal-skin-${a.skin} gift-goal-effect-${a.effect||"none"} gift-goal-mode-carousel gift-goal-transition-${a.transition||"fade"}`,style:{...E(a.position),"--goal-scale":String(a.scale),"--goal-scale-origin":$(a.position),"--goal-opacity":String(b),"--title-size":`${a.titleSize||36}px`,"--gift-size":`${a.giftSize||180}px`,"--count-size":`${a.countSize||48}px`},children:l.jsx("div",{className:"gift-goal-scale-box",children:I.map(t=>{const e=Math.max(1,Number(t.target||1)),r=Math.max(0,Number(t.current||0));return l.jsx("div",{className:"gift-goal-card",style:{"--accent":t.accent||"#ffd42a","--title-color":a.titleColor||"#ffffff","--count-color":a.countColor||"#ffffff","--target-color":a.targetColor||"#ffd42a",opacity:b},children:l.jsxs("div",{className:"gift-goal-effect-box",children:[l.jsx("div",{className:"gift-goal-label",children:a.showHeader&&a.title||"Meta"}),l.jsx("div",{className:"gift-goal-image",children:l.jsx("img",{src:t.giftImageUrl||"./events/gift.svg",alt:t.giftName})}),l.jsxs("div",{className:"gift-goal-progress",children:[N(r),l.jsxs("span",{className:"gift-goal-target",children:["/ ",N(e)]})]})]})},`${t.id}-${g}`)})})})]})}export{L as GiftGoalOverlayPage};
