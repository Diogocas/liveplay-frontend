import{i as e,n as t,t as n}from"./jsx-runtime-CY_jTiqj.js";import{p as r,u as i}from"./index-BogyQEMs.js";import{a}from"./overlay-url-DoP8AAQQ.js";import{i as o,n as s,o as c,r as l,t as u}from"./gift-goal-KXUtgoQ3.js";var d=e(t()),f=n(),p=`http://127.0.0.1:35942`;function m(e){let t=new Set,n=e=>{let n=String(e||``).trim().replace(/\/$/,``);n&&t.add(n)};return n(e),n(p),n(`https://liveplay-backend.onrender.com/overlay-bridge`),Array.from(t)}function h(e){return new Intl.NumberFormat(`pt-BR`,{notation:Number(e)>=1e4?`compact`:`standard`}).format(Math.max(0,Number(e||0)))}function g(e){let t={position:`fixed`,inset:0,width:`100vw`,height:`100vh`,display:`flex`,boxSizing:`border-box`,padding:8,overflow:`hidden`};switch(e){case`top-left`:return{...t,alignItems:`flex-start`,justifyContent:`flex-start`,transformOrigin:`top left`};case`top-right`:return{...t,alignItems:`flex-start`,justifyContent:`flex-end`,transformOrigin:`top right`};case`bottom-left`:return{...t,alignItems:`flex-end`,justifyContent:`flex-start`,transformOrigin:`bottom left`};case`bottom-right`:return{...t,alignItems:`flex-end`,justifyContent:`flex-end`,transformOrigin:`bottom right`};default:return{...t,alignItems:`center`,justifyContent:`center`,transformOrigin:`center`}}}function _(e){switch(e){case`top-left`:return`top left`;case`top-right`:return`top right`;case`bottom-left`:return`bottom left`;case`bottom-right`:return`bottom right`;default:return`center`}}function v(){let[e,t]=(0,d.useState)(()=>o()),[n,v]=(0,d.useState)(0),y=(0,d.useRef)(``),b=a((0,d.useMemo)(()=>new URLSearchParams(window.location.search),[]),`/overlay-bridge`,p),x=(0,d.useCallback)(e=>{if(e?.type!==`gift-goal-config`||!e.config)return;let n=c(e.config),r=JSON.stringify(n);r!==y.current&&(y.current=r,t(n))},[]),S=(0,d.useCallback)(e=>{let n=e?.payload??e;if(n?.type===`gift-goal-config`){x(n);return}if(n?.type===`gift-goal-event`&&n.event){t(e=>{let t=l(e,n.event);try{window.localStorage.setItem(u,JSON.stringify(t))}catch{}return r(u,t).catch(()=>void 0),t});return}n?.type===`gift-goal-reset`&&t(e=>{let t=c({...e,goals:e.goals.map(e=>({...e,current:0}))});try{window.localStorage.setItem(u,JSON.stringify(t))}catch{}return r(u,t).catch(()=>void 0),t})},[x]);(0,d.useEffect)(()=>{let e=!0;return(async()=>{let t=await i(u,null).catch(()=>null);e&&t&&x({type:`gift-goal-config`,config:t});try{let t=window.localStorage.getItem(u);e&&t&&x({type:`gift-goal-config`,config:JSON.parse(t)})}catch{}})(),()=>{e=!1}},[x]),(0,d.useEffect)(()=>{let e=!1,t=async()=>{if(!e)for(let e of m(b))try{S(await(await fetch(`${e}/snapshot?type=gift-goal-config&_=${Date.now()}`,{cache:`no-store`})).json())}catch{}};t();let n=window.setInterval(t,5e3);return()=>{e=!0,window.clearInterval(n)}},[S,b]),(0,d.useEffect)(()=>{let e=[];for(let t of m(b))for(let n of[`gift-goal-config`,`gift-goal-event`,`gift-goal-reset`])try{let r=new EventSource(`${t}/events?type=${n}`);r.onmessage=e=>{try{S(JSON.parse(e.data))}catch{}},e.push(r)}catch{}return()=>e.forEach(e=>e.close())},[S,b]),(0,d.useEffect)(()=>{let e=null;try{e=new BroadcastChannel(s),e.onmessage=e=>S(e.data)}catch{}let t=window.liveplay?.app?.onOverlaySync?.(e=>S(e));return()=>{e?.close(),typeof t==`function`&&t()}},[S]);let C=(0,d.useMemo)(()=>{let t=e.goals.filter(e=>e.enabled&&e.giftName),n=e.skipCompleted?t.filter(e=>Math.max(0,Number(e.current||0))<Math.max(1,Number(e.target||1))):t,r=n.length>0?n:t;return(e.orderMode||`closest`)===`closest`?[...r].sort((e,t)=>{let n=Math.max(0,Number(e.current||0))/Math.max(1,Number(e.target||1));return Math.max(0,Number(t.current||0))/Math.max(1,Number(t.target||1))-n}):r},[e.goals,e.skipCompleted,e.orderMode]),w=Math.max(0,Math.min(1,Number(e.opacity||94)/100)),T=Math.max(2e3,Math.min(6e4,Number(e.rotateSeconds||5)*1e3));(0,d.useEffect)(()=>{if(C.length<=1){v(0);return}v(e=>e%C.length);let e=window.setInterval(()=>v(e=>(e+1)%C.length),T);return()=>window.clearInterval(e)},[C.length,T]);let E=C.length>0?[C[n%C.length]]:[];return(0,f.jsxs)(`div`,{className:`gift-goal-overlay`,style:{opacity:+!!e.enabled,pointerEvents:`none`},children:[(0,f.jsx)(`style`,{children:`
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
        .gift-goal-transition-loop .gift-goal-card { animation: giftGoalLoop ${Math.max(1.8,Math.min(8,Number(e.rotateSeconds||5)))}s ease-in-out infinite; }
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
      `}),(0,f.jsx)(`div`,{className:`gift-goal-stack gift-goal-skin-${e.skin} gift-goal-effect-${e.effect||`none`} gift-goal-mode-carousel gift-goal-transition-${e.transition||`fade`}`,style:{...g(e.position),"--goal-scale":String(e.scale),"--goal-scale-origin":_(e.position),"--goal-opacity":String(w),"--title-size":`${e.titleSize||36}px`,"--gift-size":`${e.giftSize||180}px`,"--count-size":`${e.countSize||48}px`},children:(0,f.jsx)(`div`,{className:`gift-goal-scale-box`,children:E.map(t=>{let r=Math.max(1,Number(t.target||1)),i=Math.max(0,Number(t.current||0));return(0,f.jsx)(`div`,{className:`gift-goal-card`,style:{"--accent":t.accent||`#ffd42a`,"--title-color":e.titleColor||`#ffffff`,"--count-color":e.countColor||`#ffffff`,"--target-color":e.targetColor||`#ffd42a`,opacity:w},children:(0,f.jsxs)(`div`,{className:`gift-goal-effect-box`,children:[(0,f.jsx)(`div`,{className:`gift-goal-label`,children:e.showHeader&&e.title||`Meta`}),(0,f.jsx)(`div`,{className:`gift-goal-image`,children:(0,f.jsx)(`img`,{src:t.giftImageUrl||`./events/gift.svg`,alt:t.giftName})}),(0,f.jsxs)(`div`,{className:`gift-goal-progress`,children:[h(i),(0,f.jsxs)(`span`,{className:`gift-goal-target`,children:[`/ `,h(r)]})]})]})},`${t.id}-${n}`)})})})]})}export{v as GiftGoalOverlayPage};