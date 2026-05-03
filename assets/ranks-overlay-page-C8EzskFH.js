import{r as g,j as u}from"./index-Fw06vvaT.js";import{n as xe,c as Se,d as Y}from"./top-record-widget-DJC4u_SX.js";import{g as pe}from"./app-kv-BJKjXjFm.js";import{r as Ce}from"./overlay-url-IdlNOR_r.js";import"./brazil-gifts-DLMOKDcl.js";const Re="http://127.0.0.1:35942",F={topLike:{id:"topLike",metricIcon:"heart",configKey:"liveplay.toplike.config",snapshotKey:"liveplay.toplike.snapshot",channel:"liveplay-toplike-channel",snapshotType:"top-like-sync",rankKey:"likes",queryBoard:"topLike"},topGifts:{id:"topGifts",metricIcon:"coin",configKey:"liveplay.topgifts.config",snapshotKey:"liveplay.topgifts.snapshot",channel:"liveplay-topgifts-channel",snapshotType:"top-gifts-sync",rankKey:"gifts",queryBoard:"topGifts"},weeklyRank:{id:"weeklyRank",metricIcon:"coin",configKey:"liveplay.weeklyrank.config",snapshotKey:"liveplay.weeklyrank.snapshot",channel:"liveplay-weeklyrank-channel",snapshotType:"weekly-rank-sync",rankKey:"weekly",queryBoard:"weeklyRank"},monthlyRank:{id:"monthlyRank",metricIcon:"coin",configKey:"liveplay.monthlyrank.config",snapshotKey:"liveplay.monthlyrank.snapshot",channel:"liveplay-monthlyrank-channel",snapshotType:"monthly-rank-sync",rankKey:"monthly",queryBoard:"monthlyRank"},topGift:{id:"topGift",metricIcon:"coin",configKey:"liveplay.topgift.config",snapshotKey:"liveplay.topgift.snapshot",channel:"liveplay-topgift-channel",snapshotType:"top-gift-sync",rankKey:"topGift",queryBoard:"topGift"},topCombo:{id:"topCombo",metricIcon:"coin",configKey:"liveplay.topcombo.config",snapshotKey:"liveplay.topcombo.snapshot",channel:"liveplay-topcombo-channel",snapshotType:"top-combo-sync",rankKey:"topCombo",queryBoard:"topCombo"}};function Be(a){return a.trim().replace(/\/$/,"")}function H(a,e){return`${Be(a)}${e.startsWith("/")?e:`/${e}`}`}const f={theme:"dark",opacity:100,topCount:10,showAvatars:!0,showMedals:!0,showCrown:!0,showMetricIcon:!0,displaySeconds:8,width:820,compactMode:!1,nameFont:"display",valueFont:"display",nameColor:"#fff200",valueColor:"#22d3ee",nameFontSize:100,valueFontSize:100,nameLetterSpacing:0,valueLetterSpacing:0,lineSpacing:100,alignRight:!1},fe={display:'"Trebuchet MS", "Arial Black", Impact, sans-serif',default:'Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',tech:'"Lucida Console", "Courier New", monospace',compact:'"Arial Narrow", "Roboto Condensed", "Segoe UI", sans-serif',serif:'Georgia, "Times New Roman", serif'};function X(a){return fe[a]||fe.default}function ge(a){const e=typeof a=="string"?JSON.parse(a):a??{};return{...f,...e,topCount:e.topCount===3||e.topCount===5||e.topCount===10?e.topCount:f.topCount,theme:e.theme==="light"?"light":"dark",opacity:typeof e.opacity=="number"?Math.max(0,Math.min(100,e.opacity)):f.opacity,displaySeconds:typeof e.displaySeconds=="number"?Math.max(1,Math.min(60,e.displaySeconds)):f.displaySeconds,width:typeof e.width=="number"?Math.max(420,Math.min(1400,e.width)):f.width,nameFont:e.nameFont==="default"||e.nameFont==="display"||e.nameFont==="tech"||e.nameFont==="compact"||e.nameFont==="serif"?e.nameFont:f.nameFont,valueFont:e.valueFont==="default"||e.valueFont==="display"||e.valueFont==="tech"||e.valueFont==="compact"||e.valueFont==="serif"?e.valueFont:f.valueFont,nameColor:typeof e.nameColor=="string"&&e.nameColor.trim()?e.nameColor:f.nameColor,valueColor:typeof e.valueColor=="string"&&e.valueColor.trim()?e.valueColor:f.valueColor,nameFontSize:typeof e.nameFontSize=="number"?Math.max(70,Math.min(180,e.nameFontSize)):f.nameFontSize,valueFontSize:typeof e.valueFontSize=="number"?Math.max(70,Math.min(180,e.valueFontSize)):f.valueFontSize,nameLetterSpacing:typeof e.nameLetterSpacing=="number"?Math.max(-2,Math.min(12,e.nameLetterSpacing)):f.nameLetterSpacing,valueLetterSpacing:typeof e.valueLetterSpacing=="number"?Math.max(-2,Math.min(12,e.valueLetterSpacing)):f.valueLetterSpacing,lineSpacing:typeof e.lineSpacing=="number"?Math.max(70,Math.min(180,e.lineSpacing)):f.lineSpacing,alignRight:typeof e.alignRight=="boolean"?e.alignRight:f.alignRight}}function D(a){try{const e=window.localStorage.getItem(a.configKey);return e?ge(e):f}catch{return f}}function Z(a,e){const p=typeof e=="string"?JSON.parse(e):e??{};return{...Y(a.id),...p}}function O(a){try{const e=window.localStorage.getItem(a.configKey);return e?Z(a,e):Y(a.id)}catch{return Y(a.id)}}function Q(a){try{const e=window.localStorage.getItem(a.snapshotKey);if(!e)return null;const p=JSON.parse(e);return p.type!==a.snapshotType?null:p}catch{return null}}function Ae(a){return a.id==="topLike"?"likes":a.id==="topGifts"?"gifts":a.id==="weeklyRank"?"weeklyCoins":a.id==="monthlyRank"?"monthlyCoins":a.id==="topGift"?"topGift":"topCombo"}function Ie(a){return a==="gold"?"🥇":a==="silver"?"🥈":a==="bronze"?"🥉":null}function Fe(a){return new Intl.NumberFormat("pt-BR").format(a)}function me(a,e,p){var h,v;const t=e==="weekly"?"weeklyCoins":e==="monthly"?"monthlyCoins":e;return(Array.isArray((v=(h=a==null?void 0:a.ranks)==null?void 0:h[t])==null?void 0:v.entries)?a.ranks[t].entries:[]).slice(0,p).map((d,n)=>({position:n+1,name:String((d==null?void 0:d.nickname)||(d==null?void 0:d.username)||`Usuário ${n+1}`),value:Math.max(0,Number((d==null?void 0:d.value)||0)),avatar:String((d==null?void 0:d.avatarUrl)||"").trim()||void 0,medal:n===0?"gold":n===1?"silver":n===2?"bronze":void 0,crowned:n===0,giftId:String((d==null?void 0:d.giftId)||"").trim()||void 0,giftName:String((d==null?void 0:d.giftName)||"").trim()||void 0,giftImageUrl:String((d==null?void 0:d.giftImageUrl)||"").trim()||void 0,comboCount:Math.max(0,Number((d==null?void 0:d.comboCount)||0))||void 0,coinValue:Math.max(0,Number((d==null?void 0:d.coinValue)||0))||void 0}))}function Me(a,e){return a.map(p=>`${p.position}:${p.name}:${p.value}`).join("|")+`::${e||0}`}function $e(a,e,p,t,y=100,h="display"){return a==="coin"?u.jsx("span",{className:p?"metric-icon metric-coin event-coin":"metric-icon metric-coin",style:{color:t,fontSize:(e?17:15)*(y/100),lineHeight:1,fontFamily:X(h),filter:e?"drop-shadow(0 0 8px rgba(255,207,63,.45))":"none"},children:"●"}):u.jsx("span",{className:p?"metric-icon metric-heart event-heart":"metric-icon metric-heart",style:{color:"#ff2c55",fontSize:e?17:15,lineHeight:1,filter:e?"drop-shadow(0 0 8px rgba(255,44,85,.45))":"none"},children:"♥"})}function M(a,e){const p=Math.max(0,Math.min(1,Number(a)/100));return Math.max(0,Math.min(e,p*e))}function je(a,e){const p=M(e,a==="light"?.98:.88),t=M(e,a==="light"?.96:.1),y=M(e,a==="light"?.72:.035),h=M(e,.08),v=M(e,a==="light"?.16:.14),d=M(e,a==="light"?.18:.28);return a==="light"?{shellBackground:`rgba(248,250,252,${p})`,shellBorder:`1px solid rgba(15,23,42,${v})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${t}), rgba(226,232,240,${y}))`,rowBackground:`rgba(255,255,255,${y})`,avatarBackground:`rgba(15,23,42,${h})`,avatarInitialColor:"#0f172a",defaultNameColor:"#0f172a",defaultValueColor:"#0369a1",topHalo:"radial-gradient(circle at 12% 50%, rgba(59,130,246,.22), transparent 24%), radial-gradient(circle at 72% 50%, rgba(250,204,21,.22), transparent 42%)",shadow:d>0?`0 18px 36px rgba(15,23,42,${d})`:"none"}:{shellBackground:`rgba(4,7,18,${p})`,shellBorder:`1px solid rgba(255,255,255,${v})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${t}), rgba(255,255,255,${y}))`,rowBackground:`rgba(255,255,255,${y})`,avatarBackground:`rgba(255,255,255,${h})`,avatarInitialColor:"#d1d5db",defaultNameColor:"#fff200",defaultValueColor:"#22d3ee",topHalo:"radial-gradient(circle at 12% 50%, rgba(255,80,120,.35), transparent 24%), radial-gradient(circle at 72% 50%, rgba(255,195,0,.18), transparent 42%)",shadow:d>0?`0 18px 40px rgba(0,0,0,${d})`:"none"}}function Ke(){var se;const a=g.useMemo(()=>new URLSearchParams(window.location.search),[]),e=a.get("board")||"topLike",p=a.get("sourceId")||e,t=e==="topGifts"?F.topGifts:e==="weeklyRank"?F.weeklyRank:e==="monthlyRank"?F.monthlyRank:e==="topGift"?F.topGift:e==="topCombo"?F.topCombo:F.topLike,y=a.get("bridgeUrl"),h=typeof window<"u"&&typeof window.liveplay<"u",[v,d]=g.useState(()=>y||Ce(a,"/overlay-bridge",Re)),[n,ee]=g.useState(()=>t.id==="topGift"||t.id==="topCombo"?O(t):D(t)),[m,U]=g.useState(()=>Q(t)),[he,te]=g.useState([]),[ve,oe]=g.useState(!1),[q,ne]=g.useState(!1),P=g.useRef((m==null?void 0:m.updatedAt)??0),$=g.useRef(""),J=g.useRef(Number((n==null?void 0:n.updatedAt)||0)),_=g.useRef(!!(n!=null&&n.updatedAt)),j=r=>{if(!r){_.current||ee(t.id==="topGift"||t.id==="topCombo"?Y(t.id):f);return}const s=t.id==="topGift"||t.id==="topCombo"?Z(t,r):ge(r),l=Number((s==null?void 0:s.updatedAt)||0);if(l>0){if(l<J.current)return;J.current=l,_.current=!0}else if(_.current)return;ee(s)},T=r=>{var l,c;if(!r||r.type!=="rank-config-sync"||r.board!==t.id)return;const s=Number(r.updatedAt||((l=r.config)==null?void 0:l.updatedAt)||0);s&&s<J.current||j({...r.config,updatedAt:s||Number(((c=r.config)==null?void 0:c.updatedAt)||Date.now())})};g.useEffect(()=>{const r=document.documentElement,s=document.body,l=document.getElementById("root"),c=r.style.background,k=s.style.background,w=(l==null?void 0:l.style.background)??"";return r.style.background="transparent",s.style.background="transparent",l&&(l.style.background="transparent"),()=>{r.style.background=c,s.style.background=k,l&&(l.style.background=w)}},[]),g.useEffect(()=>{var r,s,l;h&&((l=(s=(r=window.liveplay)==null?void 0:r.app)==null?void 0:s.getOverlayBridgeUrl)==null||l.call(s).then(c=>{c!=null&&c.url&&d(c.url)}).catch(()=>{}))},[h]),g.useEffect(()=>{j(t.id==="topGift"||t.id==="topCombo"?O(t):D(t));const r=Q(t);U(r),P.current=(r==null?void 0:r.updatedAt)??0},[t]),g.useEffect(()=>{let r=!0;return(async()=>{const[l,c]=await Promise.all([pe(t.configKey,null),pe(t.snapshotKey,null)]);r&&(l&&j(l),c&&c.type===t.snapshotType&&(!c.board||c.board===t.id)&&(U(c),P.current=c.updatedAt??0))})().catch(()=>{}),()=>{r=!1}},[t]),g.useEffect(()=>{var de,V,ue;const r=()=>j(t.id==="topGift"||t.id==="topCombo"?O(t):D(t)),s=o=>{if(!o||o.type!==t.snapshotType||typeof o.board=="string"&&o.board&&o.board!==t.id)return;const i=Number(o.updatedAt||o.at||Date.now());i<P.current||(P.current=i,U({...o,updatedAt:i}))},l=o=>{o.key===t.configKey&&r(),o.key===t.snapshotKey&&s(Q(t))},c=o=>{s(o.detail)},k=o=>{T(o.detail)};window.addEventListener("storage",l),window.addEventListener(`liveplay:${t.id}-snapshot`,c),window.addEventListener(`liveplay:${t.id}-config`,k);let w=null;try{w=new BroadcastChannel(t.channel),w.onmessage=o=>{const i=o.data;if((i==null?void 0:i.type)==="rank-config-sync"){T(i);return}s(i)}}catch{}const R=window.setInterval(()=>{document.visibilityState==="visible"&&j(t.id==="topGift"||t.id==="topCombo"?O(t):D(t))},2500);let S=!1;const L=Ae(t),B=o=>{if(!o||(o==null?void 0:o.type)!=="rank-overlay-sync"||typeof(o==null?void 0:o.board)=="string"&&o.board&&o.board!==L)return;const i=o==null?void 0:o.ranks;i&&(oe(!!(o!=null&&o.liveActive)),te(me({ranks:i},t.rankKey,10)))},E=async()=>{try{const[o,i,G]=await Promise.all([fetch(H(v,`/snapshot?type=rank-config-sync&board=${t.id}&sourceId=${encodeURIComponent(p)}`),{cache:"no-store"}).then(K=>K.json()).catch(()=>null),fetch(H(v,`/snapshot?type=${t.snapshotType}&board=${t.id}&sourceId=${encodeURIComponent(p)}`),{cache:"no-store"}).then(K=>K.json()).catch(()=>null),fetch(H(v,`/snapshot?type=rank-overlay-sync&board=${L}&sourceId=${encodeURIComponent(p)}`),{cache:"no-store"}).then(K=>K.json()).catch(()=>null)]);if(S)return;const x=o==null?void 0:o.payload;(x==null?void 0:x.type)==="rank-config-sync"&&T(x);const I=i==null?void 0:i.payload;(I==null?void 0:I.type)===t.snapshotType&&s({...I,updatedAt:Number(I.updatedAt||I.at||Date.now())});const W=G==null?void 0:G.payload;(W==null?void 0:W.type)==="rank-overlay-sync"&&B(W)}catch{}};let A=null;try{A=new EventSource(H(v,"/events")),A.onmessage=o=>{try{const i=JSON.parse(o.data);if((i==null?void 0:i.type)==="rank-config-sync"){T(i);return}if((i==null?void 0:i.type)===t.snapshotType){if(typeof(i==null?void 0:i.board)=="string"&&i.board&&i.board!==t.id)return;s({...i,updatedAt:Number(i.updatedAt||i.at||Date.now())});return}(i==null?void 0:i.type)==="rank-overlay-sync"&&B(i)}catch{}}}catch{A=null}const le=(ue=(V=(de=window.liveplay)==null?void 0:de.app)==null?void 0:V.onOverlaySync)==null?void 0:ue.call(V,o=>{if((o==null?void 0:o.type)==="rank-config-sync"){T(o);return}if((o==null?void 0:o.type)===t.snapshotType){if(typeof(o==null?void 0:o.board)=="string"&&o.board&&o.board!==t.id)return;s({...o,updatedAt:Number(o.updatedAt||o.at||Date.now())});return}(o==null?void 0:o.type)==="rank-overlay-sync"&&B(o)}),ce=async()=>{var o;try{const i=(o=window.liveplay)==null?void 0:o.app;if(!(i!=null&&i.getState)||!(i!=null&&i.getStatus)){await E();return}const[G,x]=await Promise.all([i.getState(),i.getStatus()]);if(S)return;oe(!!(x!=null&&x.tiktokConnected)),te(me(G,t.rankKey,10)),await E()}catch{S||await E()}};ce();const ke=window.setInterval(()=>{document.visibilityState==="visible"&&ce()},900);return()=>{S=!0,window.removeEventListener("storage",l),window.removeEventListener(`liveplay:${t.id}-snapshot`,c),window.removeEventListener(`liveplay:${t.id}-config`,k),window.clearInterval(R),window.clearInterval(ke),w==null||w.close(),A==null||A.close(),typeof le=="function"&&le()}},[v,h,t,p]);const ae=t.id==="topGift"||t.id==="topCombo"?1:n.topCount,N=!!(m!=null&&m.visible)&&m.mode==="test"&&((se=m.entries)==null?void 0:se.length),C=N?m.entries.slice(0,ae):ve?he.slice(0,ae):[];if(g.useEffect(()=>{if(!N||!m)return;const r=t.id==="topGift"||t.id==="topCombo"?8:Math.max(1,Math.min(60,Number(n.displaySeconds||f.displaySeconds))),s=window.setTimeout(()=>{U(l=>!l||l.mode!=="test"||l.updatedAt!==m.updatedAt?l:{...l,visible:!1,updatedAt:Date.now()})},r*1e3+450);return()=>window.clearTimeout(s)},[N,m==null?void 0:m.updatedAt,t.id,n]),g.useEffect(()=>{const r=Me(C,N?m==null?void 0:m.updatedAt:0);if(!r){$.current="";return}if($.current&&$.current!==r){ne(!0);const s=window.setTimeout(()=>ne(!1),950);return $.current=r,()=>window.clearTimeout(s)}$.current=r},[C,N,m==null?void 0:m.updatedAt]),(t.id==="topGift"||t.id==="topCombo")&&C.length){const r=Z(t,n),s=C[0],l=xe({username:s.name,nickname:s.name,avatarUrl:s.avatar,giftId:s.giftId,giftName:s.giftName,giftImageUrl:s.giftImageUrl,comboCount:s.comboCount,coinValue:s.coinValue,value:s.value},t.id);return l?u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:"flex-start",padding:0,overflow:"hidden"},children:u.jsx("div",{style:{width:390,maxWidth:"100%",padding:10,boxSizing:"border-box",background:"transparent",border:"none",borderRadius:16,boxShadow:"none"},children:u.jsx(Se,{kind:t.id,config:r,entry:l,embedded:!0})})}):u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}})}if(!C.length)return u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}});const be=n.compactMode?8:10,z=n.compactMode?42:48,re=n.compactMode?30:34,ie=(n.compactMode?13:15)*(n.nameFontSize/100),we=(n.compactMode?12:14)*(n.valueFontSize/100),ye=2*(n.lineSpacing/100),b=je(n.theme,n.opacity);return u.jsxs(u.Fragment,{children:[u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:n.alignRight?"flex-end":"flex-start",padding:0,overflow:"hidden"},children:u.jsx("div",{style:{width:Math.min(n.width,1400),minHeight:180,borderRadius:16,border:b.shellBorder,background:b.shellBackground,padding:n.compactMode?"8px 10px":"10px 12px",boxSizing:"border-box",overflow:"hidden",boxShadow:b.shadow,isolation:"isolate"},children:u.jsx("div",{style:{display:"grid",gap:be,maxWidth:"100%"},children:C.map((r,s)=>{const l=Ie(r.medal),c=s===0,k=s===1,w=s===2,R=q&&(c||k||w),S=u.jsxs("div",{style:{minWidth:0,display:"grid",gap:ye,position:"relative",zIndex:2,justifyItems:n.alignRight?"end":"start"},children:[u.jsx("div",{style:{color:n.theme==="light"&&n.nameColor===f.nameColor?b.defaultNameColor:n.nameColor,fontFamily:X(n.nameFont),fontWeight:c?900:800,fontSize:c?ie+2:ie,lineHeight:1.1,letterSpacing:`${n.nameLetterSpacing}px`,whiteSpace:"nowrap",overflow:"hidden",textOverflow:"ellipsis",textShadow:c?"0 0 10px rgba(255,242,0,.28)":"none"},children:r.name}),u.jsxs("div",{style:{display:"flex",alignItems:"center",gap:6,justifyContent:n.alignRight?"flex-end":"flex-start",color:n.theme==="light"&&n.valueColor===f.valueColor?b.defaultValueColor:n.valueColor,fontFamily:X(n.valueFont),fontWeight:800,fontSize:we,letterSpacing:`${n.valueLetterSpacing}px`},children:[n.showMetricIcon?$e(t.metricIcon,c,R,n.theme==="light"&&n.valueColor===f.valueColor?b.defaultValueColor:n.valueColor,n.valueFontSize,n.valueFont):null,u.jsx("span",{className:R?"value-bump":"",style:{display:"inline-block",textShadow:c?"0 0 10px rgba(34,211,238,.22)":"none"},children:Fe(r.value)})]})]}),L=n.showAvatars?u.jsxs("div",{style:{position:"relative",width:z,height:z,borderRadius:"50%",overflow:"visible",border:c?"2px solid rgba(255,215,0,.95)":"2px solid rgba(255,193,7,.75)",background:b.avatarBackground,display:"grid",placeItems:"center",zIndex:2,boxShadow:c?"0 0 18px rgba(255,210,0,.45)":k||w?"0 0 12px rgba(255,255,255,.12)":"none"},children:[r.avatar?u.jsx("img",{src:r.avatar,alt:r.name,style:{width:"100%",height:"100%",objectFit:"cover",borderRadius:"50%"},referrerPolicy:"no-referrer"}):u.jsx("span",{style:{color:b.avatarInitialColor,fontSize:Math.max(18,z*.42),fontWeight:800},children:r.name.charAt(0).toUpperCase()}),n.showCrown&&r.crowned?u.jsx("span",{className:q?"crown crown-bounce":"crown",style:{position:"absolute",top:-18,left:8,fontSize:n.compactMode?22:24,zIndex:3,filter:"drop-shadow(0 0 10px rgba(255,200,0,.55))"},children:"👑"}):null]}):null,B=n.showMedals?u.jsx("div",{style:{color:c?"#ffb72e":r.position<=3?"#ffffff":"#ff3434",fontSize:c?24:r.position<=3?20:32,fontWeight:900,lineHeight:1,textAlign:"center",position:"relative",zIndex:2,textShadow:c?"0 0 12px rgba(255,183,46,.65)":"none"},children:l||u.jsxs("span",{style:{display:"inline-block",minWidth:20},children:[r.position,"."]})}):null,E=n.alignRight?`minmax(0, 1fr)${n.showAvatars?` ${z}px`:""}${n.showMedals?` ${re}px`:""}`:`${n.showMedals?`${re}px `:""}${n.showAvatars?`${z}px `:""}minmax(0, 1fr)`;return u.jsxs("div",{className:["lp-rank-row",c?"top1":"",k?"top2":"",w?"top3":"",R?"event-pulse":""].join(" ").trim(),style:{display:"grid",gridTemplateColumns:E,alignItems:"center",columnGap:10,minHeight:n.compactMode?46:54,padding:c?"6px 8px":"4px 6px",borderRadius:14,background:c?b.rowBackgroundTop:b.rowBackground,border:n.theme==="light"?"1px solid rgba(15,23,42,.06)":"1px solid rgba(255,255,255,.045)",position:"relative",transform:R&&c?"scale(1.035)":"scale(1)",transition:"transform 220ms ease, filter 220ms ease"},children:[c?u.jsx("div",{className:q?"top1-halo pulse":"top1-halo",style:{position:"absolute",inset:"2px 2px 2px 2px",borderRadius:16,pointerEvents:"none",background:b.topHalo,filter:"blur(8px)",opacity:.95}}):null,n.alignRight?u.jsxs(u.Fragment,{children:[S,L,B]}):u.jsxs(u.Fragment,{children:[B,L,S]})]},`${r.position}-${r.name}-${r.value}`)})})})}),u.jsx("style",{children:`
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
