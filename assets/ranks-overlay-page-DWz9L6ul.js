import{r as f,j as p}from"./index-PsxuCPEE.js";import{n as ge,c as ve,d as G}from"./top-record-widget-CyUN69R-.js";import{g as re}from"./app-kv-BJKjXjFm.js";import{r as ye}from"./overlay-url-BLL8b4A3.js";import"./brazil-gifts-N5Kr9nKr.js";const we="http://127.0.0.1:35942",R={topLike:{id:"topLike",metricIcon:"heart",configKey:"liveplay.toplike.config",snapshotKey:"liveplay.toplike.snapshot",channel:"liveplay-toplike-channel",snapshotType:"top-like-sync",rankKey:"likes",queryBoard:"topLike"},topGifts:{id:"topGifts",metricIcon:"coin",configKey:"liveplay.topgifts.config",snapshotKey:"liveplay.topgifts.snapshot",channel:"liveplay-topgifts-channel",snapshotType:"top-gifts-sync",rankKey:"gifts",queryBoard:"topGifts"},weeklyRank:{id:"weeklyRank",metricIcon:"coin",configKey:"liveplay.weeklyrank.config",snapshotKey:"liveplay.weeklyrank.snapshot",channel:"liveplay-weeklyrank-channel",snapshotType:"weekly-rank-sync",rankKey:"weekly",queryBoard:"weeklyRank"},monthlyRank:{id:"monthlyRank",metricIcon:"coin",configKey:"liveplay.monthlyrank.config",snapshotKey:"liveplay.monthlyrank.snapshot",channel:"liveplay-monthlyrank-channel",snapshotType:"monthly-rank-sync",rankKey:"monthly",queryBoard:"monthlyRank"},topGift:{id:"topGift",metricIcon:"coin",configKey:"liveplay.topgift.config",snapshotKey:"liveplay.topgift.snapshot",channel:"liveplay-topgift-channel",snapshotType:"top-gift-sync",rankKey:"topGift",queryBoard:"topGift"},topCombo:{id:"topCombo",metricIcon:"coin",configKey:"liveplay.topcombo.config",snapshotKey:"liveplay.topcombo.snapshot",channel:"liveplay-topcombo-channel",snapshotType:"top-combo-sync",rankKey:"topCombo",queryBoard:"topCombo"}};function be(a){return a.trim().replace(/\/$/,"")}function O(a,e){return`${be(a)}${e.startsWith("/")?e:`/${e}`}`}const u={theme:"dark",opacity:100,topCount:10,showAvatars:!0,showMedals:!0,showCrown:!0,showMetricIcon:!0,displaySeconds:8,width:820,compactMode:!1,nameFont:"display",valueFont:"display",nameColor:"#fff200",valueColor:"#22d3ee",nameFontSize:100,valueFontSize:100,nameLetterSpacing:0,valueLetterSpacing:0,lineSpacing:100,alignRight:!1},se={display:'"Trebuchet MS", "Arial Black", Impact, sans-serif',default:'Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',tech:'"Lucida Console", "Courier New", monospace',compact:'"Arial Narrow", "Roboto Condensed", "Segoe UI", sans-serif',serif:'Georgia, "Times New Roman", serif'};function H(a){return se[a]||se.default}function le(a){const e=typeof a=="string"?JSON.parse(a):a??{};return{...u,...e,topCount:e.topCount===3||e.topCount===5||e.topCount===10?e.topCount:u.topCount,theme:e.theme==="light"?"light":"dark",opacity:typeof e.opacity=="number"?Math.max(0,Math.min(100,e.opacity)):u.opacity,displaySeconds:typeof e.displaySeconds=="number"?Math.max(1,Math.min(60,e.displaySeconds)):u.displaySeconds,width:typeof e.width=="number"?Math.max(420,Math.min(1400,e.width)):u.width,nameFont:e.nameFont==="default"||e.nameFont==="display"||e.nameFont==="tech"||e.nameFont==="compact"||e.nameFont==="serif"?e.nameFont:u.nameFont,valueFont:e.valueFont==="default"||e.valueFont==="display"||e.valueFont==="tech"||e.valueFont==="compact"||e.valueFont==="serif"?e.valueFont:u.valueFont,nameColor:typeof e.nameColor=="string"&&e.nameColor.trim()?e.nameColor:u.nameColor,valueColor:typeof e.valueColor=="string"&&e.valueColor.trim()?e.valueColor:u.valueColor,nameFontSize:typeof e.nameFontSize=="number"?Math.max(70,Math.min(180,e.nameFontSize)):u.nameFontSize,valueFontSize:typeof e.valueFontSize=="number"?Math.max(70,Math.min(180,e.valueFontSize)):u.valueFontSize,nameLetterSpacing:typeof e.nameLetterSpacing=="number"?Math.max(-2,Math.min(12,e.nameLetterSpacing)):u.nameLetterSpacing,valueLetterSpacing:typeof e.valueLetterSpacing=="number"?Math.max(-2,Math.min(12,e.valueLetterSpacing)):u.valueLetterSpacing,lineSpacing:typeof e.lineSpacing=="number"?Math.max(70,Math.min(180,e.lineSpacing)):u.lineSpacing,alignRight:typeof e.alignRight=="boolean"?e.alignRight:u.alignRight}}function K(a){try{const e=window.localStorage.getItem(a.configKey);return e?le(e):u}catch{return u}}function q(a,e){const m=typeof e=="string"?JSON.parse(e):e??{};return{...G(a.id),...m}}function ke(a){try{const e=window.localStorage.getItem(a.configKey);return e?q(a,e):G(a.id)}catch{return G(a.id)}}function Y(a){try{const e=window.localStorage.getItem(a.snapshotKey);if(!e)return null;const m=JSON.parse(e);return m.type!==a.snapshotType?null:m}catch{return null}}function Se(a){return a.id==="topLike"?"likes":a.id==="topGifts"?"gifts":a.id==="weeklyRank"?"weeklyCoins":a.id==="monthlyRank"?"monthlyCoins":a.id==="topGift"?"topGift":"topCombo"}function xe(a){return a==="gold"?"🥇":a==="silver"?"🥈":a==="bronze"?"🥉":null}function Ce(a){return new Intl.NumberFormat("pt-BR").format(a)}function ce(a,e,m){var y,b;const n=e==="weekly"?"weeklyCoins":e==="monthly"?"monthlyCoins":e;return(Array.isArray((b=(y=a==null?void 0:a.ranks)==null?void 0:y[n])==null?void 0:b.entries)?a.ranks[n].entries:[]).slice(0,m).map((c,o)=>({position:o+1,name:String((c==null?void 0:c.nickname)||(c==null?void 0:c.username)||`Usuário ${o+1}`),value:Math.max(0,Number((c==null?void 0:c.value)||0)),avatar:String((c==null?void 0:c.avatarUrl)||"").trim()||void 0,medal:o===0?"gold":o===1?"silver":o===2?"bronze":void 0,crowned:o===0,giftId:String((c==null?void 0:c.giftId)||"").trim()||void 0,giftName:String((c==null?void 0:c.giftName)||"").trim()||void 0,giftImageUrl:String((c==null?void 0:c.giftImageUrl)||"").trim()||void 0,comboCount:Math.max(0,Number((c==null?void 0:c.comboCount)||0))||void 0,coinValue:Math.max(0,Number((c==null?void 0:c.coinValue)||0))||void 0}))}function Re(a){return a.map(e=>`${e.position}:${e.name}:${e.value}`).join("|")}function Ie(a,e,m,n,L=100,y="display"){return a==="coin"?p.jsx("span",{className:m?"metric-icon metric-coin event-coin":"metric-icon metric-coin",style:{color:n,fontSize:(e?17:15)*(L/100),lineHeight:1,fontFamily:H(y),filter:e?"drop-shadow(0 0 8px rgba(255,207,63,.45))":"none"},children:"●"}):p.jsx("span",{className:m?"metric-icon metric-heart event-heart":"metric-icon metric-heart",style:{color:"#ff2c55",fontSize:e?17:15,lineHeight:1,filter:e?"drop-shadow(0 0 8px rgba(255,44,85,.45))":"none"},children:"♥"})}function Te(){var ne;const a=f.useMemo(()=>new URLSearchParams(window.location.search),[]),e=a.get("board")||"topLike",m=a.get("sourceId")||e,n=e==="topGifts"?R.topGifts:e==="weeklyRank"?R.weeklyRank:e==="monthlyRank"?R.monthlyRank:e==="topGift"?R.topGift:e==="topCombo"?R.topCombo:R.topLike,L=a.get("bridgeUrl"),y=typeof window<"u"&&typeof window.liveplay<"u",[b,c]=f.useState(()=>L||ye(a,"/overlay-bridge",we)),[o,D]=f.useState(()=>n.id==="topGift"||n.id==="topCombo"?ke(n):K(n)),[g,U]=f.useState(()=>Y(n)),[pe,_]=f.useState([]),[de,J]=f.useState(!1),[P,Q]=f.useState(!1),T=f.useRef((g==null?void 0:g.updatedAt)??0),I=f.useRef(""),W=f.useRef(Number((o==null?void 0:o.updatedAt)||0)),V=f.useRef(!!(o!=null&&o.updatedAt)),F=i=>{if(!i){V.current||D(n.id==="topGift"||n.id==="topCombo"?G(n.id):u);return}const r=n.id==="topGift"||n.id==="topCombo"?q(n,i):le(i),l=Number((r==null?void 0:r.updatedAt)||0);if(l>0){if(l<W.current)return;W.current=l,V.current=!0}else if(V.current)return;D(r)},X=i=>{var l,s;if(!i||i.type!=="rank-config-sync"||i.board!==n.id)return;const r=Number(i.updatedAt||((l=i.config)==null?void 0:l.updatedAt)||0);r&&r<W.current||F({...i.config,updatedAt:r||Number(((s=i.config)==null?void 0:s.updatedAt)||Date.now())})};f.useEffect(()=>{const i=document.documentElement,r=document.body,l=document.getElementById("root"),s=i.style.background,h=r.style.background,k=(l==null?void 0:l.style.background)??"";return i.style.background="transparent",r.style.background="transparent",l&&(l.style.background="transparent"),()=>{i.style.background=s,r.style.background=h,l&&(l.style.background=k)}},[]),f.useEffect(()=>{var i,r,l;y&&((l=(r=(i=window.liveplay)==null?void 0:i.app)==null?void 0:r.getOverlayBridgeUrl)==null||l.call(r).then(s=>{s!=null&&s.url&&c(s.url)}).catch(()=>{}))},[y]),f.useEffect(()=>{F(K(n));const i=Y(n);U(i),T.current=(i==null?void 0:i.updatedAt)??0},[n]),f.useEffect(()=>{let i=!0;return(async()=>{const[l,s]=await Promise.all([re(n.configKey,null),re(n.snapshotKey,null)]);i&&(l&&F(l),s&&s.type===n.snapshotType&&(!s.board||s.board===n.id)&&(U(s),T.current=s.updatedAt??0))})().catch(()=>{}),()=>{i=!1}},[n]),f.useEffect(()=>{var ie,$,ae;const i=()=>F(K(n)),r=t=>{if(!t||t.type!==n.snapshotType||typeof t.board=="string"&&t.board&&t.board!==n.id)return;const d=Number(t.updatedAt||t.at||0);d<T.current||(T.current=d,U({...t,updatedAt:d}))},l=t=>{t.key===n.configKey&&i(),t.key===n.snapshotKey&&r(Y(n))},s=t=>{r(t.detail)};window.addEventListener("storage",l),window.addEventListener(`liveplay:${n.id}-snapshot`,s);let h=null;try{h=new BroadcastChannel(n.channel),h.onmessage=t=>r(t.data)}catch{}const k=window.setInterval(()=>{document.visibilityState==="visible"&&F(K(n))},2500);let v=!1;const j=Se(n),A=t=>{if(!t||(t==null?void 0:t.type)!=="rank-overlay-sync"||typeof(t==null?void 0:t.board)=="string"&&t.board&&t.board!==j)return;const d=t==null?void 0:t.ranks;d&&(J(!!(t!=null&&t.liveActive)),_(ce({ranks:d},n.rankKey,10)))},x=async()=>{try{const[t,d,B]=await Promise.all([fetch(O(b,`/snapshot?type=rank-config-sync&board=${n.id}&sourceId=${encodeURIComponent(m)}`),{cache:"no-store"}).then(z=>z.json()).catch(()=>null),fetch(O(b,`/snapshot?type=${n.snapshotType}&board=${n.id}&sourceId=${encodeURIComponent(m)}`),{cache:"no-store"}).then(z=>z.json()).catch(()=>null),fetch(O(b,`/snapshot?type=rank-overlay-sync&board=${j}&sourceId=${encodeURIComponent(m)}`),{cache:"no-store"}).then(z=>z.json()).catch(()=>null)]);if(v)return;const w=t==null?void 0:t.payload;(w==null?void 0:w.type)==="rank-config-sync"&&X(w);const C=d==null?void 0:d.payload;(C==null?void 0:C.type)===n.snapshotType&&r({...C,updatedAt:Number(C.updatedAt||C.at||Date.now())});const E=B==null?void 0:B.payload;(E==null?void 0:E.type)==="rank-overlay-sync"&&A(E)}catch{}},N=(ae=($=(ie=window.liveplay)==null?void 0:ie.app)==null?void 0:$.onOverlaySync)==null?void 0:ae.call($,t=>{if((t==null?void 0:t.type)==="rank-config-sync"){X(t);return}if((t==null?void 0:t.type)===n.snapshotType){if(typeof(t==null?void 0:t.board)=="string"&&t.board&&t.board!==n.id)return;r({...t,updatedAt:Number(t.updatedAt||t.at||Date.now())});return}(t==null?void 0:t.type)==="rank-overlay-sync"&&A(t)}),oe=async()=>{var t;try{const d=(t=window.liveplay)==null?void 0:t.app;if(!(d!=null&&d.getState)||!(d!=null&&d.getStatus)){await x();return}const[B,w]=await Promise.all([d.getState(),d.getStatus()]);if(v)return;J(!!(w!=null&&w.tiktokConnected)),_(ce(B,n.rankKey,10)),await x()}catch{v||await x()}};oe();const he=window.setInterval(()=>{document.visibilityState==="visible"&&oe()},900);return()=>{v=!0,window.removeEventListener("storage",l),window.removeEventListener(`liveplay:${n.id}-snapshot`,s),window.clearInterval(k),window.clearInterval(he),h==null||h.close(),typeof N=="function"&&N()}},[b,y,n,m]);const Z=n.id==="topGift"||n.id==="topCombo"?1:o.topCount,S=!!(g!=null&&g.visible)&&g.mode==="test"&&((ne=g.entries)==null?void 0:ne.length)?g.entries.slice(0,Z):de?pe.slice(0,Z):[];if(f.useEffect(()=>{const i=Re(S);if(!i){I.current="";return}if(I.current&&I.current!==i){Q(!0);const r=window.setTimeout(()=>Q(!1),950);return I.current=i,()=>window.clearTimeout(r)}I.current=i},[S]),(n.id==="topGift"||n.id==="topCombo")&&S.length){const i=q(n,o),r=S[0],l=ge({username:r.name,nickname:r.name,avatarUrl:r.avatar,giftId:r.giftId,giftName:r.giftName,giftImageUrl:r.giftImageUrl,comboCount:r.comboCount,coinValue:r.coinValue,value:r.value},n.id);return l?p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:"flex-start",padding:0,overflow:"hidden"},children:p.jsx("div",{style:{width:390,maxWidth:"100%",padding:10,boxSizing:"border-box"},children:p.jsx(ve,{kind:n.id,config:i,entry:l,embedded:!0})})}):p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}})}if(!S.length)return p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}});const ue=o.compactMode?8:10,M=o.compactMode?42:48,ee=o.compactMode?30:34,te=(o.compactMode?13:15)*(o.nameFontSize/100),fe=(o.compactMode?12:14)*(o.valueFontSize/100),me=2*(o.lineSpacing/100);return p.jsxs(p.Fragment,{children:[p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:o.alignRight?"flex-end":"flex-start",padding:0,overflow:"hidden"},children:p.jsx("div",{style:{width:Math.min(o.width,1400),minHeight:180,borderRadius:0,border:"none",background:"transparent",padding:o.compactMode?"8px 10px":"10px 12px",boxSizing:"border-box",overflow:"hidden",boxShadow:"none"},children:p.jsx("div",{style:{display:"grid",gap:ue,maxWidth:"100%"},children:S.map((i,r)=>{const l=xe(i.medal),s=r===0,h=r===1,k=r===2,v=P&&(s||h||k),j=p.jsxs("div",{style:{minWidth:0,display:"grid",gap:me,position:"relative",zIndex:2,justifyItems:o.alignRight?"end":"start"},children:[p.jsx("div",{style:{color:o.nameColor,fontFamily:H(o.nameFont),fontWeight:s?900:800,fontSize:s?te+2:te,lineHeight:1.1,letterSpacing:`${o.nameLetterSpacing}px`,whiteSpace:"nowrap",overflow:"hidden",textOverflow:"ellipsis",textShadow:s?"0 0 10px rgba(255,242,0,.28)":"none"},children:i.name}),p.jsxs("div",{style:{display:"flex",alignItems:"center",gap:6,justifyContent:o.alignRight?"flex-end":"flex-start",color:o.valueColor,fontFamily:H(o.valueFont),fontWeight:800,fontSize:fe,letterSpacing:`${o.valueLetterSpacing}px`},children:[o.showMetricIcon?Ie(n.metricIcon,s,v,o.valueColor,o.valueFontSize,o.valueFont):null,p.jsx("span",{className:v?"value-bump":"",style:{display:"inline-block",textShadow:s?"0 0 10px rgba(34,211,238,.22)":"none"},children:Ce(i.value)})]})]}),A=o.showAvatars?p.jsxs("div",{style:{position:"relative",width:M,height:M,borderRadius:"50%",overflow:"visible",border:s?"2px solid rgba(255,215,0,.95)":"2px solid rgba(255,193,7,.75)",background:"rgba(255,255,255,.06)",display:"grid",placeItems:"center",zIndex:2,boxShadow:s?"0 0 18px rgba(255,210,0,.45)":h||k?"0 0 12px rgba(255,255,255,.12)":"none"},children:[i.avatar?p.jsx("img",{src:i.avatar,alt:i.name,style:{width:"100%",height:"100%",objectFit:"cover",borderRadius:"50%"},referrerPolicy:"no-referrer"}):p.jsx("span",{style:{color:"#d1d5db",fontSize:Math.max(18,M*.42),fontWeight:800},children:i.name.charAt(0).toUpperCase()}),o.showCrown&&i.crowned?p.jsx("span",{className:P?"crown crown-bounce":"crown",style:{position:"absolute",top:-18,left:8,fontSize:o.compactMode?22:24,zIndex:3,filter:"drop-shadow(0 0 10px rgba(255,200,0,.55))"},children:"👑"}):null]}):null,x=o.showMedals?p.jsx("div",{style:{color:s?"#ffb72e":i.position<=3?"#ffffff":"#ff3434",fontSize:s?24:i.position<=3?20:32,fontWeight:900,lineHeight:1,textAlign:"center",position:"relative",zIndex:2,textShadow:s?"0 0 12px rgba(255,183,46,.65)":"none"},children:l||p.jsxs("span",{style:{display:"inline-block",minWidth:20},children:[i.position,"."]})}):null,N=o.alignRight?`minmax(0, 1fr)${o.showAvatars?` ${M}px`:""}${o.showMedals?` ${ee}px`:""}`:`${o.showMedals?`${ee}px `:""}${o.showAvatars?`${M}px `:""}minmax(0, 1fr)`;return p.jsxs("div",{className:["lp-rank-row",s?"top1":"",h?"top2":"",k?"top3":"",v?"event-pulse":""].join(" ").trim(),style:{display:"grid",gridTemplateColumns:N,alignItems:"center",columnGap:10,minHeight:o.compactMode?46:54,padding:s?"6px 8px":"4px 6px",borderRadius:14,background:"transparent",position:"relative",transform:v&&s?"scale(1.035)":"scale(1)",transition:"transform 220ms ease, filter 220ms ease",opacity:Math.max(0,Math.min(1,o.opacity/100))},children:[s?p.jsx("div",{className:P?"top1-halo pulse":"top1-halo",style:{position:"absolute",inset:"2px 2px 2px 2px",borderRadius:16,pointerEvents:"none",background:"radial-gradient(circle at 12% 50%, rgba(255,80,120,.35), transparent 24%), radial-gradient(circle at 72% 50%, rgba(255,195,0,.18), transparent 40%)",filter:"blur(8px)",opacity:.95}}):null,o.alignRight?p.jsxs(p.Fragment,{children:[j,A,x]}):p.jsxs(p.Fragment,{children:[x,A,j]})]},`${i.position}-${i.name}-${i.value}`)})})})}),p.jsx("style",{children:`
        .lp-rank-row.top1 {
          will-change: transform, filter;
        }

        .lp-rank-row.top2,
        .lp-rank-row.top3 {
          will-change: transform, filter;
        }

        .lp-rank-row.event-pulse {
          animation: rowEventPulse 780ms ease;
        }

        .top1-halo {
          animation: haloBreath 2.1s ease-in-out infinite;
        }

        .top1-halo.pulse {
          animation: haloBurst 820ms ease, haloBreath 2.1s ease-in-out 860ms infinite;
        }

        .crown {
          transform-origin: center;
        }

        .crown.crown-bounce {
          animation: crownBounce 880ms ease;
        }

        .metric-heart.event-heart,
        .metric-coin.event-coin {
          animation: metricPulse 680ms ease;
        }

        .value-bump {
          animation: valueBump 720ms ease;
        }

        @keyframes rowEventPulse {
          0% {
            transform: scale(1);
            filter: brightness(1);
          }
          30% {
            transform: scale(1.04);
            filter: brightness(1.16);
          }
          100% {
            transform: scale(1);
            filter: brightness(1);
          }
        }

        @keyframes haloBreath {
          0% {
            opacity: 0.72;
            transform: scale(0.98);
          }
          50% {
            opacity: 1;
            transform: scale(1.015);
          }
          100% {
            opacity: 0.72;
            transform: scale(0.98);
          }
        }

        @keyframes haloBurst {
          0% {
            opacity: 0.5;
            transform: scale(0.94);
          }
          45% {
            opacity: 1;
            transform: scale(1.06);
          }
          100% {
            opacity: 0.88;
            transform: scale(1);
          }
        }

        @keyframes crownBounce {
          0% {
            transform: translateY(-1px) scale(1);
          }
          25% {
            transform: translateY(-7px) scale(1.12) rotate(-8deg);
          }
          55% {
            transform: translateY(0) scale(0.98) rotate(4deg);
          }
          100% {
            transform: translateY(0) scale(1) rotate(0deg);
          }
        }

        @keyframes metricPulse {
          0% {
            transform: scale(1);
          }
          35% {
            transform: scale(1.34);
          }
          100% {
            transform: scale(1);
          }
        }

        @keyframes valueBump {
          0% {
            transform: translateY(0) scale(1);
            color: #22d3ee;
          }
          35% {
            transform: translateY(-2px) scale(1.16);
            color: #7df9ff;
          }
          100% {
            transform: translateY(0) scale(1);
            color: #22d3ee;
          }
        }
      `})]})}export{Te as RanksOverlayPage};
