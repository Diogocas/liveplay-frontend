import{r as g,j as d}from"./index-CDAoMpV3.js";import{n as ke,c as xe,d as H}from"./top-record-widget-DwY3C819.js";import{g as de}from"./app-kv-BJKjXjFm.js";import{r as Se}from"./overlay-url-IdlNOR_r.js";import"./brazil-gifts-DLMOKDcl.js";const Ce="http://127.0.0.1:35942",I={topLike:{id:"topLike",metricIcon:"heart",configKey:"liveplay.toplike.config",snapshotKey:"liveplay.toplike.snapshot",channel:"liveplay-toplike-channel",snapshotType:"top-like-sync",rankKey:"likes",queryBoard:"topLike"},topGifts:{id:"topGifts",metricIcon:"coin",configKey:"liveplay.topgifts.config",snapshotKey:"liveplay.topgifts.snapshot",channel:"liveplay-topgifts-channel",snapshotType:"top-gifts-sync",rankKey:"gifts",queryBoard:"topGifts"},weeklyRank:{id:"weeklyRank",metricIcon:"coin",configKey:"liveplay.weeklyrank.config",snapshotKey:"liveplay.weeklyrank.snapshot",channel:"liveplay-weeklyrank-channel",snapshotType:"weekly-rank-sync",rankKey:"weekly",queryBoard:"weeklyRank"},monthlyRank:{id:"monthlyRank",metricIcon:"coin",configKey:"liveplay.monthlyrank.config",snapshotKey:"liveplay.monthlyrank.snapshot",channel:"liveplay-monthlyrank-channel",snapshotType:"monthly-rank-sync",rankKey:"monthly",queryBoard:"monthlyRank"},topGift:{id:"topGift",metricIcon:"coin",configKey:"liveplay.topgift.config",snapshotKey:"liveplay.topgift.snapshot",channel:"liveplay-topgift-channel",snapshotType:"top-gift-sync",rankKey:"topGift",queryBoard:"topGift"},topCombo:{id:"topCombo",metricIcon:"coin",configKey:"liveplay.topcombo.config",snapshotKey:"liveplay.topcombo.snapshot",channel:"liveplay-topcombo-channel",snapshotType:"top-combo-sync",rankKey:"topCombo",queryBoard:"topCombo"}};function Re(a){return a.trim().replace(/\/$/,"")}function q(a,e){return`${Re(a)}${e.startsWith("/")?e:`/${e}`}`}const f={theme:"dark",opacity:100,topCount:10,showAvatars:!0,showMedals:!0,showCrown:!0,showMetricIcon:!0,displaySeconds:8,width:820,compactMode:!1,nameFont:"display",valueFont:"display",nameColor:"#fff200",valueColor:"#22d3ee",nameFontSize:100,valueFontSize:100,nameLetterSpacing:0,valueLetterSpacing:0,lineSpacing:100,alignRight:!1},ue={display:'"Trebuchet MS", "Arial Black", Impact, sans-serif',default:'Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',tech:'"Lucida Console", "Courier New", monospace',compact:'"Arial Narrow", "Roboto Condensed", "Segoe UI", sans-serif',serif:'Georgia, "Times New Roman", serif'};function J(a){return ue[a]||ue.default}function me(a){const e=typeof a=="string"?JSON.parse(a):a??{};return{...f,...e,topCount:e.topCount===3||e.topCount===5||e.topCount===10?e.topCount:f.topCount,theme:e.theme==="light"?"light":"dark",opacity:typeof e.opacity=="number"?Math.max(0,Math.min(100,e.opacity)):f.opacity,displaySeconds:typeof e.displaySeconds=="number"?Math.max(1,Math.min(60,e.displaySeconds)):f.displaySeconds,width:typeof e.width=="number"?Math.max(420,Math.min(1400,e.width)):f.width,nameFont:e.nameFont==="default"||e.nameFont==="display"||e.nameFont==="tech"||e.nameFont==="compact"||e.nameFont==="serif"?e.nameFont:f.nameFont,valueFont:e.valueFont==="default"||e.valueFont==="display"||e.valueFont==="tech"||e.valueFont==="compact"||e.valueFont==="serif"?e.valueFont:f.valueFont,nameColor:typeof e.nameColor=="string"&&e.nameColor.trim()?e.nameColor:f.nameColor,valueColor:typeof e.valueColor=="string"&&e.valueColor.trim()?e.valueColor:f.valueColor,nameFontSize:typeof e.nameFontSize=="number"?Math.max(70,Math.min(180,e.nameFontSize)):f.nameFontSize,valueFontSize:typeof e.valueFontSize=="number"?Math.max(70,Math.min(180,e.valueFontSize)):f.valueFontSize,nameLetterSpacing:typeof e.nameLetterSpacing=="number"?Math.max(-2,Math.min(12,e.nameLetterSpacing)):f.nameLetterSpacing,valueLetterSpacing:typeof e.valueLetterSpacing=="number"?Math.max(-2,Math.min(12,e.valueLetterSpacing)):f.valueLetterSpacing,lineSpacing:typeof e.lineSpacing=="number"?Math.max(70,Math.min(180,e.lineSpacing)):f.lineSpacing,alignRight:typeof e.alignRight=="boolean"?e.alignRight:f.alignRight}}function W(a){try{const e=window.localStorage.getItem(a.configKey);return e?me(e):f}catch{return f}}function Q(a,e){const p=typeof e=="string"?JSON.parse(e):e??{};return{...H(a.id),...p}}function Be(a){try{const e=window.localStorage.getItem(a.configKey);return e?Q(a,e):H(a.id)}catch{return H(a.id)}}function _(a){try{const e=window.localStorage.getItem(a.snapshotKey);if(!e)return null;const p=JSON.parse(e);return p.type!==a.snapshotType?null:p}catch{return null}}function Ie(a){return a.id==="topLike"?"likes":a.id==="topGifts"?"gifts":a.id==="weeklyRank"?"weeklyCoins":a.id==="monthlyRank"?"monthlyCoins":a.id==="topGift"?"topGift":"topCombo"}function Ae(a){return a==="gold"?"🥇":a==="silver"?"🥈":a==="bronze"?"🥉":null}function Fe(a){return new Intl.NumberFormat("pt-BR").format(a)}function pe(a,e,p){var h,w;const o=e==="weekly"?"weeklyCoins":e==="monthly"?"monthlyCoins":e;return(Array.isArray((w=(h=a==null?void 0:a.ranks)==null?void 0:h[o])==null?void 0:w.entries)?a.ranks[o].entries:[]).slice(0,p).map((c,t)=>({position:t+1,name:String((c==null?void 0:c.nickname)||(c==null?void 0:c.username)||`Usuário ${t+1}`),value:Math.max(0,Number((c==null?void 0:c.value)||0)),avatar:String((c==null?void 0:c.avatarUrl)||"").trim()||void 0,medal:t===0?"gold":t===1?"silver":t===2?"bronze":void 0,crowned:t===0,giftId:String((c==null?void 0:c.giftId)||"").trim()||void 0,giftName:String((c==null?void 0:c.giftName)||"").trim()||void 0,giftImageUrl:String((c==null?void 0:c.giftImageUrl)||"").trim()||void 0,comboCount:Math.max(0,Number((c==null?void 0:c.comboCount)||0))||void 0,coinValue:Math.max(0,Number((c==null?void 0:c.coinValue)||0))||void 0}))}function Me(a,e){return a.map(p=>`${p.position}:${p.name}:${p.value}`).join("|")+`::${e||0}`}function $e(a,e,p,o,y=100,h="display"){return a==="coin"?d.jsx("span",{className:p?"metric-icon metric-coin event-coin":"metric-icon metric-coin",style:{color:o,fontSize:(e?17:15)*(y/100),lineHeight:1,fontFamily:J(h),filter:e?"drop-shadow(0 0 8px rgba(255,207,63,.45))":"none"},children:"●"}):d.jsx("span",{className:p?"metric-icon metric-heart event-heart":"metric-icon metric-heart",style:{color:"#ff2c55",fontSize:e?17:15,lineHeight:1,filter:e?"drop-shadow(0 0 8px rgba(255,44,85,.45))":"none"},children:"♥"})}function A(a,e){const p=Math.max(0,Math.min(1,Number(a)/100));return Math.max(0,Math.min(e,p*e))}function fe(a,e){const p=A(e,a==="light"?.98:.88),o=A(e,a==="light"?.96:.1),y=A(e,a==="light"?.72:.035),h=A(e,.08),w=A(e,a==="light"?.16:.14),c=A(e,a==="light"?.18:.28);return a==="light"?{shellBackground:`rgba(248,250,252,${p})`,shellBorder:`1px solid rgba(15,23,42,${w})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${o}), rgba(226,232,240,${y}))`,rowBackground:`rgba(255,255,255,${y})`,avatarBackground:`rgba(15,23,42,${h})`,avatarInitialColor:"#0f172a",defaultNameColor:"#0f172a",defaultValueColor:"#0369a1",topHalo:"radial-gradient(circle at 12% 50%, rgba(59,130,246,.22), transparent 24%), radial-gradient(circle at 72% 50%, rgba(250,204,21,.22), transparent 42%)",shadow:c>0?`0 18px 36px rgba(15,23,42,${c})`:"none"}:{shellBackground:`rgba(4,7,18,${p})`,shellBorder:`1px solid rgba(255,255,255,${w})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${o}), rgba(255,255,255,${y}))`,rowBackground:`rgba(255,255,255,${y})`,avatarBackground:`rgba(255,255,255,${h})`,avatarInitialColor:"#d1d5db",defaultNameColor:"#fff200",defaultValueColor:"#22d3ee",topHalo:"radial-gradient(circle at 12% 50%, rgba(255,80,120,.35), transparent 24%), radial-gradient(circle at 72% 50%, rgba(255,195,0,.18), transparent 42%)",shadow:c>0?`0 18px 40px rgba(0,0,0,${c})`:"none"}}function Ke(){var re;const a=g.useMemo(()=>new URLSearchParams(window.location.search),[]),e=a.get("board")||"topLike",p=a.get("sourceId")||e,o=e==="topGifts"?I.topGifts:e==="weeklyRank"?I.weeklyRank:e==="monthlyRank"?I.monthlyRank:e==="topGift"?I.topGift:e==="topCombo"?I.topCombo:I.topLike,y=a.get("bridgeUrl"),h=typeof window<"u"&&typeof window.liveplay<"u",[w,c]=g.useState(()=>y||Se(a,"/overlay-bridge",Ce)),[t,X]=g.useState(()=>o.id==="topGift"||o.id==="topCombo"?Be(o):W(o)),[m,K]=g.useState(()=>_(o)),[ge,Z]=g.useState([]),[he,ee]=g.useState(!1),[D,te]=g.useState(!1),G=g.useRef((m==null?void 0:m.updatedAt)??0),F=g.useRef(""),O=g.useRef(Number((t==null?void 0:t.updatedAt)||0)),Y=g.useRef(!!(t!=null&&t.updatedAt)),M=r=>{if(!r){Y.current||X(o.id==="topGift"||o.id==="topCombo"?H(o.id):f);return}const i=o.id==="topGift"||o.id==="topCombo"?Q(o,r):me(r),l=Number((i==null?void 0:i.updatedAt)||0);if(l>0){if(l<O.current)return;O.current=l,Y.current=!0}else if(Y.current)return;X(i)},U=r=>{var l,s;if(!r||r.type!=="rank-config-sync"||r.board!==o.id)return;const i=Number(r.updatedAt||((l=r.config)==null?void 0:l.updatedAt)||0);i&&i<O.current||M({...r.config,updatedAt:i||Number(((s=r.config)==null?void 0:s.updatedAt)||Date.now())})};g.useEffect(()=>{const r=document.documentElement,i=document.body,l=document.getElementById("root"),s=r.style.background,k=i.style.background,b=(l==null?void 0:l.style.background)??"";return r.style.background="transparent",i.style.background="transparent",l&&(l.style.background="transparent"),()=>{r.style.background=s,i.style.background=k,l&&(l.style.background=b)}},[]),g.useEffect(()=>{var r,i,l;h&&((l=(i=(r=window.liveplay)==null?void 0:r.app)==null?void 0:i.getOverlayBridgeUrl)==null||l.call(i).then(s=>{s!=null&&s.url&&c(s.url)}).catch(()=>{}))},[h]),g.useEffect(()=>{M(W(o));const r=_(o);K(r),G.current=(r==null?void 0:r.updatedAt)??0},[o]),g.useEffect(()=>{let r=!0;return(async()=>{const[l,s]=await Promise.all([de(o.configKey,null),de(o.snapshotKey,null)]);r&&(l&&M(l),s&&s.type===o.snapshotType&&(!s.board||s.board===o.id)&&(K(s),G.current=s.updatedAt??0))})().catch(()=>{}),()=>{r=!1}},[o]),g.useEffect(()=>{var le,P,ce;const r=()=>M(W(o)),i=n=>{if(!n||n.type!==o.snapshotType||typeof n.board=="string"&&n.board&&n.board!==o.id)return;const u=Number(n.updatedAt||n.at||Date.now());u<G.current||(G.current=u,K({...n,updatedAt:u}))},l=n=>{n.key===o.configKey&&r(),n.key===o.snapshotKey&&i(_(o))},s=n=>{i(n.detail)},k=n=>{U(n.detail)};window.addEventListener("storage",l),window.addEventListener(`liveplay:${o.id}-snapshot`,s),window.addEventListener(`liveplay:${o.id}-config`,k);let b=null;try{b=new BroadcastChannel(o.channel),b.onmessage=n=>{const u=n.data;if((u==null?void 0:u.type)==="rank-config-sync"){U(u);return}i(u)}}catch{}const R=window.setInterval(()=>{document.visibilityState==="visible"&&M(W(o))},2500);let S=!1;const T=Ie(o),z=n=>{if(!n||(n==null?void 0:n.type)!=="rank-overlay-sync"||typeof(n==null?void 0:n.board)=="string"&&n.board&&n.board!==T)return;const u=n==null?void 0:n.ranks;u&&(ee(!!(n!=null&&n.liveActive)),Z(pe({ranks:u},o.rankKey,10)))},N=async()=>{try{const[n,u,L]=await Promise.all([fetch(q(w,`/snapshot?type=rank-config-sync&board=${o.id}&sourceId=${encodeURIComponent(p)}`),{cache:"no-store"}).then(E=>E.json()).catch(()=>null),fetch(q(w,`/snapshot?type=${o.snapshotType}&board=${o.id}&sourceId=${encodeURIComponent(p)}`),{cache:"no-store"}).then(E=>E.json()).catch(()=>null),fetch(q(w,`/snapshot?type=rank-overlay-sync&board=${T}&sourceId=${encodeURIComponent(p)}`),{cache:"no-store"}).then(E=>E.json()).catch(()=>null)]);if(S)return;const x=n==null?void 0:n.payload;(x==null?void 0:x.type)==="rank-config-sync"&&U(x);const B=u==null?void 0:u.payload;(B==null?void 0:B.type)===o.snapshotType&&i({...B,updatedAt:Number(B.updatedAt||B.at||Date.now())});const V=L==null?void 0:L.payload;(V==null?void 0:V.type)==="rank-overlay-sync"&&z(V)}catch{}},ie=(ce=(P=(le=window.liveplay)==null?void 0:le.app)==null?void 0:P.onOverlaySync)==null?void 0:ce.call(P,n=>{if((n==null?void 0:n.type)==="rank-config-sync"){U(n);return}if((n==null?void 0:n.type)===o.snapshotType){if(typeof(n==null?void 0:n.board)=="string"&&n.board&&n.board!==o.id)return;i({...n,updatedAt:Number(n.updatedAt||n.at||Date.now())});return}(n==null?void 0:n.type)==="rank-overlay-sync"&&z(n)}),se=async()=>{var n;try{const u=(n=window.liveplay)==null?void 0:n.app;if(!(u!=null&&u.getState)||!(u!=null&&u.getStatus)){await N();return}const[L,x]=await Promise.all([u.getState(),u.getStatus()]);if(S)return;ee(!!(x!=null&&x.tiktokConnected)),Z(pe(L,o.rankKey,10)),await N()}catch{S||await N()}};se();const ye=window.setInterval(()=>{document.visibilityState==="visible"&&se()},900);return()=>{S=!0,window.removeEventListener("storage",l),window.removeEventListener(`liveplay:${o.id}-snapshot`,s),window.removeEventListener(`liveplay:${o.id}-config`,k),window.clearInterval(R),window.clearInterval(ye),b==null||b.close(),typeof ie=="function"&&ie()}},[w,h,o,p]);const oe=o.id==="topGift"||o.id==="topCombo"?1:t.topCount,$=!!(m!=null&&m.visible)&&m.mode==="test"&&((re=m.entries)==null?void 0:re.length),C=$?m.entries.slice(0,oe):he?ge.slice(0,oe):[];if(g.useEffect(()=>{if(!$||!m)return;const r=o.id==="topGift"||o.id==="topCombo"?8:Math.max(1,Math.min(60,Number(t.displaySeconds||f.displaySeconds))),i=window.setTimeout(()=>{K(l=>!l||l.mode!=="test"||l.updatedAt!==m.updatedAt?l:{...l,visible:!1,updatedAt:Date.now()})},r*1e3+450);return()=>window.clearTimeout(i)},[$,m==null?void 0:m.updatedAt,o.id,t]),g.useEffect(()=>{const r=Me(C,$?m==null?void 0:m.updatedAt:0);if(!r){F.current="";return}if(F.current&&F.current!==r){te(!0);const i=window.setTimeout(()=>te(!1),950);return F.current=r,()=>window.clearTimeout(i)}F.current=r},[C,$,m==null?void 0:m.updatedAt]),(o.id==="topGift"||o.id==="topCombo")&&C.length){const r=Q(o,t),i=C[0],l=ke({username:i.name,nickname:i.name,avatarUrl:i.avatar,giftId:i.giftId,giftName:i.giftName,giftImageUrl:i.giftImageUrl,comboCount:i.comboCount,coinValue:i.coinValue,value:i.value},o.id);if(!l)return d.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}});const s=fe(t.theme==="light"?"light":"dark",Number(t.opacity??100));return d.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:"flex-start",padding:0,overflow:"hidden"},children:d.jsx("div",{style:{width:390,maxWidth:"100%",padding:10,boxSizing:"border-box",background:s.shellBackground,border:s.shellBorder,borderRadius:16,boxShadow:s.shadow},children:d.jsx(xe,{kind:o.id,config:r,entry:l,embedded:!0})})})}if(!C.length)return d.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}});const ve=t.compactMode?8:10,j=t.compactMode?42:48,ne=t.compactMode?30:34,ae=(t.compactMode?13:15)*(t.nameFontSize/100),be=(t.compactMode?12:14)*(t.valueFontSize/100),we=2*(t.lineSpacing/100),v=fe(t.theme,t.opacity);return d.jsxs(d.Fragment,{children:[d.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:t.alignRight?"flex-end":"flex-start",padding:0,overflow:"hidden"},children:d.jsx("div",{style:{width:Math.min(t.width,1400),minHeight:180,borderRadius:16,border:v.shellBorder,background:v.shellBackground,padding:t.compactMode?"8px 10px":"10px 12px",boxSizing:"border-box",overflow:"hidden",boxShadow:v.shadow,isolation:"isolate"},children:d.jsx("div",{style:{display:"grid",gap:ve,maxWidth:"100%"},children:C.map((r,i)=>{const l=Ae(r.medal),s=i===0,k=i===1,b=i===2,R=D&&(s||k||b),S=d.jsxs("div",{style:{minWidth:0,display:"grid",gap:we,position:"relative",zIndex:2,justifyItems:t.alignRight?"end":"start"},children:[d.jsx("div",{style:{color:t.theme==="light"&&t.nameColor===f.nameColor?v.defaultNameColor:t.nameColor,fontFamily:J(t.nameFont),fontWeight:s?900:800,fontSize:s?ae+2:ae,lineHeight:1.1,letterSpacing:`${t.nameLetterSpacing}px`,whiteSpace:"nowrap",overflow:"hidden",textOverflow:"ellipsis",textShadow:s?"0 0 10px rgba(255,242,0,.28)":"none"},children:r.name}),d.jsxs("div",{style:{display:"flex",alignItems:"center",gap:6,justifyContent:t.alignRight?"flex-end":"flex-start",color:t.theme==="light"&&t.valueColor===f.valueColor?v.defaultValueColor:t.valueColor,fontFamily:J(t.valueFont),fontWeight:800,fontSize:be,letterSpacing:`${t.valueLetterSpacing}px`},children:[t.showMetricIcon?$e(o.metricIcon,s,R,t.theme==="light"&&t.valueColor===f.valueColor?v.defaultValueColor:t.valueColor,t.valueFontSize,t.valueFont):null,d.jsx("span",{className:R?"value-bump":"",style:{display:"inline-block",textShadow:s?"0 0 10px rgba(34,211,238,.22)":"none"},children:Fe(r.value)})]})]}),T=t.showAvatars?d.jsxs("div",{style:{position:"relative",width:j,height:j,borderRadius:"50%",overflow:"visible",border:s?"2px solid rgba(255,215,0,.95)":"2px solid rgba(255,193,7,.75)",background:v.avatarBackground,display:"grid",placeItems:"center",zIndex:2,boxShadow:s?"0 0 18px rgba(255,210,0,.45)":k||b?"0 0 12px rgba(255,255,255,.12)":"none"},children:[r.avatar?d.jsx("img",{src:r.avatar,alt:r.name,style:{width:"100%",height:"100%",objectFit:"cover",borderRadius:"50%"},referrerPolicy:"no-referrer"}):d.jsx("span",{style:{color:v.avatarInitialColor,fontSize:Math.max(18,j*.42),fontWeight:800},children:r.name.charAt(0).toUpperCase()}),t.showCrown&&r.crowned?d.jsx("span",{className:D?"crown crown-bounce":"crown",style:{position:"absolute",top:-18,left:8,fontSize:t.compactMode?22:24,zIndex:3,filter:"drop-shadow(0 0 10px rgba(255,200,0,.55))"},children:"👑"}):null]}):null,z=t.showMedals?d.jsx("div",{style:{color:s?"#ffb72e":r.position<=3?"#ffffff":"#ff3434",fontSize:s?24:r.position<=3?20:32,fontWeight:900,lineHeight:1,textAlign:"center",position:"relative",zIndex:2,textShadow:s?"0 0 12px rgba(255,183,46,.65)":"none"},children:l||d.jsxs("span",{style:{display:"inline-block",minWidth:20},children:[r.position,"."]})}):null,N=t.alignRight?`minmax(0, 1fr)${t.showAvatars?` ${j}px`:""}${t.showMedals?` ${ne}px`:""}`:`${t.showMedals?`${ne}px `:""}${t.showAvatars?`${j}px `:""}minmax(0, 1fr)`;return d.jsxs("div",{className:["lp-rank-row",s?"top1":"",k?"top2":"",b?"top3":"",R?"event-pulse":""].join(" ").trim(),style:{display:"grid",gridTemplateColumns:N,alignItems:"center",columnGap:10,minHeight:t.compactMode?46:54,padding:s?"6px 8px":"4px 6px",borderRadius:14,background:s?v.rowBackgroundTop:v.rowBackground,border:t.theme==="light"?"1px solid rgba(15,23,42,.06)":"1px solid rgba(255,255,255,.045)",position:"relative",transform:R&&s?"scale(1.035)":"scale(1)",transition:"transform 220ms ease, filter 220ms ease"},children:[s?d.jsx("div",{className:D?"top1-halo pulse":"top1-halo",style:{position:"absolute",inset:"2px 2px 2px 2px",borderRadius:16,pointerEvents:"none",background:v.topHalo,filter:"blur(8px)",opacity:.95}}):null,t.alignRight?d.jsxs(d.Fragment,{children:[S,T,z]}):d.jsxs(d.Fragment,{children:[z,T,S]})]},`${r.position}-${r.name}-${r.value}`)})})})}),d.jsx("style",{children:`
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
      `})]})}export{Ke as RanksOverlayPage};
