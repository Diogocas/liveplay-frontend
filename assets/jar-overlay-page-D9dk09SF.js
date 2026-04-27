import{r as i,j as s}from"./index-CZZemPmQ.js";function S(n){return new Intl.NumberFormat("pt-BR").format(n||0)}function E(){const[n,h]=i.useState([]),[o,y]=i.useState([]),[j,l]=i.useState([]),c=i.useRef(0),b=Math.min(1,o.length/50);i.useEffect(()=>{var t,e;if((e=(t=window.liveplay)==null?void 0:t.app)!=null&&e.onOverlaySync)return window.liveplay.app.onOverlaySync(r=>{var p,d,m,g;if((r==null?void 0:r.type)!=="jar-overlay-sync")return;h(((p=r.state)==null?void 0:p.entries)||[]),y(((d=r.state)==null?void 0:d.recentGifts)||[]);const f=((m=r.state)==null?void 0:m.burstNonce)||0;if(f!==c.current){c.current=f;const x=(g=r.state)==null?void 0:g.lastGift;if(!x)return;const u={id:Date.now().toString(),giftImageUrl:x.giftImageUrl,left:40+Math.random()*20,size:40+Math.random()*20,rotate:-20+Math.random()*40};l(a=>[...a,u]),setTimeout(()=>{l(a=>a.filter(N=>N.id!==u.id))},1200)}})},[]);const v=i.useMemo(()=>[...n].sort((t,e)=>e.coins-t.coins).slice(0,5),[n]),w={position:"fixed",top:"50%",left:"50%",transform:"translate(-50%, -50%)",pointerEvents:"none"};return s.jsxs("div",{style:w,children:[s.jsx("style",{children:`
        html,body { background: transparent !important }

        .jar-root {
          display:flex;
          flex-direction:column;
          align-items:center;
          gap:20px;
        }

        .jar-glass {
          width:300px;
          height:380px;
          border-radius:80px;
          border:6px solid rgba(255,255,255,.5);
          position:relative;
          overflow:hidden;
          background:rgba(255,255,255,.05);
        }

        .jar-fill {
          position:absolute;
          bottom:0;
          width:100%;
          height:${b*100}%;
          background:linear-gradient(#5ea3ff,#7c8cff);
          opacity:.25;
          transition:.6s;
        }

        .gift {
          position:absolute;
          width:30px;
          height:30px;
        }

        .falling {
          position:absolute;
          top:-40px;
          animation:fall 1s ease forwards;
        }

        @keyframes fall {
          0% { transform:translateY(-100px) scale(.6); opacity:0 }
          50% { opacity:1 }
          100% { transform:translateY(300px); opacity:0 }
        }

        .top {
          display:flex;
          flex-direction:column;
          gap:6px;
          width:260px;
        }

        .row {
          display:flex;
          justify-content:space-between;
          background:rgba(0,0,0,.6);
          padding:6px 10px;
          border-radius:10px;
          color:#fff;
          font-size:12px;
        }
      `}),s.jsxs("div",{className:"jar-root",children:[s.jsxs("div",{className:"jar-glass",children:[s.jsx("div",{className:"jar-fill"}),o.map((t,e)=>s.jsx("img",{src:t.giftImageUrl||"./events/gift.svg",className:"gift",style:{left:`${10+e%5*18}%`,bottom:`${e%10*8}%`}},e))]}),j.map(t=>s.jsx("img",{src:t.giftImageUrl||"./events/gift.svg",className:"falling",style:{left:`${t.left}%`,width:t.size,transform:`rotate(${t.rotate}deg)`}},t.id)),s.jsx("div",{className:"top",children:v.map((t,e)=>s.jsxs("div",{className:"row",children:[s.jsxs("span",{children:[e+1,"º ",t.nickname]}),s.jsx("span",{children:S(t.coins)})]},e))})]})]})}export{E as JarOverlayPage};
