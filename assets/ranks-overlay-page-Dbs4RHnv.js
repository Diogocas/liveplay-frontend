import{r as h,j as p}from"./index-B0fvlV2T.js";import{n as Me,c as Te,d as X}from"./top-record-widget-DXO8I1ki.js";import{g as he}from"./app-kv-DDdMRHVm.js";import{r as je}from"./overlay-url-IdlNOR_r.js";import"./brazil-gifts-DLMOKDcl.js";const Ee="http://127.0.0.1:35942",E={topLike:{id:"topLike",metricIcon:"heart",configKey:"liveplay.toplike.config",snapshotKey:"liveplay.toplike.snapshot",channel:"liveplay-toplike-channel",snapshotType:"top-like-sync",rankKey:"likes",queryBoard:"topLike"},topGifts:{id:"topGifts",metricIcon:"coin",configKey:"liveplay.topgifts.config",snapshotKey:"liveplay.topgifts.snapshot",channel:"liveplay-topgifts-channel",snapshotType:"top-gifts-sync",rankKey:"gifts",queryBoard:"topGifts"},weeklyRank:{id:"weeklyRank",metricIcon:"coin",configKey:"liveplay.weeklyrank.config",snapshotKey:"liveplay.weeklyrank.snapshot",channel:"liveplay-weeklyrank-channel",snapshotType:"weekly-rank-sync",rankKey:"weekly",queryBoard:"weeklyRank"},monthlyRank:{id:"monthlyRank",metricIcon:"coin",configKey:"liveplay.monthlyrank.config",snapshotKey:"liveplay.monthlyrank.snapshot",channel:"liveplay-monthlyrank-channel",snapshotType:"monthly-rank-sync",rankKey:"monthly",queryBoard:"monthlyRank"},topGift:{id:"topGift",metricIcon:"coin",configKey:"liveplay.topgift.config",snapshotKey:"liveplay.topgift.snapshot",channel:"liveplay-topgift-channel",snapshotType:"top-gift-sync",rankKey:"topGift",queryBoard:"topGift"},topCombo:{id:"topCombo",metricIcon:"coin",configKey:"liveplay.topcombo.config",snapshotKey:"liveplay.topcombo.snapshot",channel:"liveplay-topcombo-channel",snapshotType:"top-combo-sync",rankKey:"topCombo",queryBoard:"topCombo"}};function Ne(n){return n.trim().replace(/\/$/,"")}function J(n,e){return`${Ne(n)}${e.startsWith("/")?e:`/${e}`}`}const f={theme:"dark",opacity:100,topCount:10,showAvatars:!0,showMedals:!0,showCrown:!0,showMetricIcon:!0,displaySeconds:8,width:820,compactMode:!1,nameFont:"display",valueFont:"display",nameColor:"#fff200",valueColor:"#22d3ee",nameFontSize:100,valueFontSize:100,nameLetterSpacing:0,valueLetterSpacing:0,lineSpacing:100,alignRight:!1},ye={display:'"Trebuchet MS", "Arial Black", Impact, sans-serif',default:'Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',tech:'"Lucida Console", "Courier New", monospace',compact:'"Arial Narrow", "Roboto Condensed", "Segoe UI", sans-serif',serif:'Georgia, "Times New Roman", serif'};function ae(n){return ye[n]||ye.default}function be(n){const e=typeof n=="string"?JSON.parse(n):n??{};return{...f,...e,topCount:e.topCount===3||e.topCount===5||e.topCount===10?e.topCount:f.topCount,theme:e.theme==="light"?"light":"dark",opacity:typeof e.opacity=="number"?Math.max(0,Math.min(100,e.opacity)):f.opacity,displaySeconds:typeof e.displaySeconds=="number"?Math.max(1,Math.min(60,e.displaySeconds)):f.displaySeconds,width:typeof e.width=="number"?Math.max(420,Math.min(1400,e.width)):f.width,nameFont:e.nameFont==="default"||e.nameFont==="display"||e.nameFont==="tech"||e.nameFont==="compact"||e.nameFont==="serif"?e.nameFont:f.nameFont,valueFont:e.valueFont==="default"||e.valueFont==="display"||e.valueFont==="tech"||e.valueFont==="compact"||e.valueFont==="serif"?e.valueFont:f.valueFont,nameColor:typeof e.nameColor=="string"&&e.nameColor.trim()?e.nameColor:f.nameColor,valueColor:typeof e.valueColor=="string"&&e.valueColor.trim()?e.valueColor:f.valueColor,nameFontSize:typeof e.nameFontSize=="number"?Math.max(70,Math.min(180,e.nameFontSize)):f.nameFontSize,valueFontSize:typeof e.valueFontSize=="number"?Math.max(70,Math.min(180,e.valueFontSize)):f.valueFontSize,nameLetterSpacing:typeof e.nameLetterSpacing=="number"?Math.max(-2,Math.min(12,e.nameLetterSpacing)):f.nameLetterSpacing,valueLetterSpacing:typeof e.valueLetterSpacing=="number"?Math.max(-2,Math.min(12,e.valueLetterSpacing)):f.valueLetterSpacing,lineSpacing:typeof e.lineSpacing=="number"?Math.max(70,Math.min(180,e.lineSpacing)):f.lineSpacing,alignRight:typeof e.alignRight=="boolean"?e.alignRight:f.alignRight}}function _(n){try{const e=window.localStorage.getItem(n.configKey);return e?be(e):f}catch{return f}}function re(n,e){const u=typeof e=="string"?JSON.parse(e):e??{};return{...X(n.id),...u}}function Q(n){try{const e=window.localStorage.getItem(n.configKey);return e?re(n,e):X(n.id)}catch{return X(n.id)}}function ne(n){try{const e=window.localStorage.getItem(n.snapshotKey);if(!e)return null;const u=JSON.parse(e);return u.type!==n.snapshotType?null:u}catch{return null}}function ve(n){return n.id==="topLike"?"likes":n.id==="topGifts"?"gifts":n.id==="weeklyRank"?"weeklyCoins":n.id==="monthlyRank"?"monthlyCoins":n.id==="topGift"?"topGift":"topCombo"}function ze(n){return n==="gold"?"🥇":n==="silver"?"🥈":n==="bronze"?"🥉":null}function Le(n){return new Intl.NumberFormat("pt-BR").format(n)}function we(n,e,u){var k,S;const t=e==="weekly"?"weeklyCoins":e==="monthly"?"monthlyCoins":e;return(Array.isArray((S=(k=n==null?void 0:n.ranks)==null?void 0:k[t])==null?void 0:S.entries)?n.ranks[t].entries:[]).slice(0,u).map((c,y)=>({position:y+1,name:String((c==null?void 0:c.nickname)||(c==null?void 0:c.username)||`Usuário ${y+1}`),value:Math.max(0,Number((c==null?void 0:c.value)||0)),avatar:String((c==null?void 0:c.avatarUrl)||"").trim()||void 0,medal:y===0?"gold":y===1?"silver":y===2?"bronze":void 0,crowned:y===0,giftId:String((c==null?void 0:c.giftId)||"").trim()||void 0,giftName:String((c==null?void 0:c.giftName)||"").trim()||void 0,giftImageUrl:String((c==null?void 0:c.giftImageUrl)||"").trim()||void 0,comboCount:Math.max(0,Number((c==null?void 0:c.comboCount)||0))||void 0,coinValue:Math.max(0,Number((c==null?void 0:c.coinValue)||0))||void 0}))}function Ue(n,e){return n.map(u=>`${u.position}:${u.name}:${u.value}`).join("|")+`::${e||0}`}function Ke(n,e,u,t,R=100,k="display"){return n==="coin"?p.jsx("span",{className:u?"metric-icon metric-coin event-coin":"metric-icon metric-coin",style:{color:t,fontSize:(e?17:15)*(R/100),lineHeight:1,fontFamily:ae(k),filter:e?"drop-shadow(0 0 8px rgba(255,207,63,.45))":"none"},children:"●"}):p.jsx("span",{className:u?"metric-icon metric-heart event-heart":"metric-icon metric-heart",style:{color:"#ff2c55",fontSize:e?17:15,lineHeight:1,filter:e?"drop-shadow(0 0 8px rgba(255,44,85,.45))":"none"},children:"♥"})}function N(n,e){const u=Math.max(0,Math.min(1,Number(n)/100));return Math.max(0,Math.min(e,u*e))}function Ge(n,e){const u=N(e,n==="light"?.98:.88),t=N(e,n==="light"?.96:.1),R=N(e,n==="light"?.72:.035),k=N(e,.08),S=N(e,n==="light"?.16:.14),c=N(e,n==="light"?.18:.28);return n==="light"?{shellBackground:`rgba(248,250,252,${u})`,shellBorder:`1px solid rgba(15,23,42,${S})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${t}), rgba(226,232,240,${R}))`,rowBackground:`rgba(255,255,255,${R})`,avatarBackground:`rgba(15,23,42,${k})`,avatarInitialColor:"#0f172a",defaultNameColor:"#0f172a",defaultValueColor:"#0369a1",topHalo:"radial-gradient(circle at 12% 50%, rgba(59,130,246,.22), transparent 24%), radial-gradient(circle at 72% 50%, rgba(250,204,21,.22), transparent 42%)",shadow:c>0?`0 18px 36px rgba(15,23,42,${c})`:"none"}:{shellBackground:`rgba(4,7,18,${u})`,shellBorder:`1px solid rgba(255,255,255,${S})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${t}), rgba(255,255,255,${R}))`,rowBackground:`rgba(255,255,255,${R})`,avatarBackground:`rgba(255,255,255,${k})`,avatarInitialColor:"#d1d5db",defaultNameColor:"#fff200",defaultValueColor:"#22d3ee",topHalo:"radial-gradient(circle at 12% 50%, rgba(255,80,120,.35), transparent 24%), radial-gradient(circle at 72% 50%, rgba(255,195,0,.18), transparent 42%)",shadow:c>0?`0 18px 40px rgba(0,0,0,${c})`:"none"}}function Ye(){var me;const n=h.useMemo(()=>new URLSearchParams(window.location.search),[]),e=n.get("board")||"topLike",u=n.get("sourceId")||e,t=e==="topGifts"?E.topGifts:e==="weeklyRank"?E.weeklyRank:e==="monthlyRank"?E.monthlyRank:e==="topGift"?E.topGift:e==="topCombo"?E.topCombo:E.topLike,R=n.get("bridgeUrl"),k=typeof window<"u"&&typeof window.liveplay<"u",[S,c]=h.useState(()=>R||je(n,"/overlay-bridge",Ee)),[y,ie]=h.useState(()=>t.id==="topGift"||t.id==="topCombo"?Q(t):_(t)),[g,H]=h.useState(()=>ne(t)),[ke,se]=h.useState([]),[Se,le]=h.useState(!1),[Z,ce]=h.useState(!1),[Ce,xe]=h.useState(()=>new Set),O=h.useRef((g==null?void 0:g.updatedAt)??0),z=h.useRef(""),ee=h.useRef(Number((y==null?void 0:y.updatedAt)||0)),te=h.useRef(!!(y!=null&&y.updatedAt)),L=o=>{if(!o){te.current||ie(t.id==="topGift"||t.id==="topCombo"?X(t.id):f);return}const r=t.id==="topGift"||t.id==="topCombo"?re(t,o):be(o),s=Number((r==null?void 0:r.updatedAt)||0);if(s>0){if(s<ee.current)return;ee.current=s,te.current=!0}else if(te.current)return;ie(r)},I=o=>o!=null&&o.payload&&typeof o.payload=="object"?o.payload:o,oe=(o,r)=>{const s=String((o==null?void 0:o.board)||"").trim(),l=String((o==null?void 0:o.sourceId)||"").trim(),b=new Set([t.id,t.queryBoard,ve(t),r||"",u].filter(Boolean));return!(s&&!b.has(s)||l&&!b.has(l))},U=o=>{var l,b;const r=I(o);if(!r||r.type!=="rank-config-sync"||!oe(r))return;const s=Number(r.updatedAt||((l=r.config)==null?void 0:l.updatedAt)||0);s&&s<ee.current||L({...r.config,updatedAt:s||Number(((b=r.config)==null?void 0:b.updatedAt)||Date.now())})};h.useEffect(()=>{const o=document.documentElement,r=document.body,s=document.getElementById("root"),l=o.style.background,b=r.style.background,x=(s==null?void 0:s.style.background)??"";return o.style.background="transparent",r.style.background="transparent",s&&(s.style.background="transparent"),()=>{o.style.background=l,r.style.background=b,s&&(s.style.background=x)}},[]),h.useEffect(()=>{var o,r,s;k&&((s=(r=(o=window.liveplay)==null?void 0:o.app)==null?void 0:r.getOverlayBridgeUrl)==null||s.call(r).then(l=>{l!=null&&l.url&&c(l.url)}).catch(()=>{}))},[k]),h.useEffect(()=>{L(t.id==="topGift"||t.id==="topCombo"?Q(t):_(t));const o=ne(t);H(o),O.current=(o==null?void 0:o.updatedAt)??0},[t]),h.useEffect(()=>{let o=!0;return(async()=>{const[s,l]=await Promise.all([he(t.configKey,null),he(t.snapshotKey,null)]);o&&(s&&L(s),l&&l.type===t.snapshotType&&(!l.board||l.board===t.id)&&(H(l),O.current=l.updatedAt??0))})().catch(()=>{}),()=>{o=!1}},[t]),h.useEffect(()=>{var fe,Y,ge;const o=()=>L(t.id==="topGift"||t.id==="topCombo"?Q(t):_(t)),r=d=>{const a=I(d);if(!a||a.type!==t.snapshotType||!oe(a))return;d=a;const w=Number(d.updatedAt||d.at||Date.now());w<O.current||(O.current=w,H({...d,updatedAt:w}))},s=d=>{d.key===t.configKey&&o(),d.key===t.snapshotKey&&r(ne(t))},l=d=>{r(d.detail)},b=d=>{U(d.detail)};window.addEventListener("storage",s),window.addEventListener(`liveplay:${t.id}-snapshot`,l),window.addEventListener(`liveplay:${t.id}-config`,b);let x=null;try{x=new BroadcastChannel(t.channel),x.onmessage=d=>{const a=I(d.data);if((a==null?void 0:a.type)==="rank-config-sync"){U(a);return}r(a)}}catch{}const F=window.setInterval(()=>{document.visibilityState==="visible"&&L(t.id==="topGift"||t.id==="topCombo"?Q(t):_(t))},2500);let B=!1;const G=ve(t),M=d=>{const a=I(d);if(!a||(a==null?void 0:a.type)!=="rank-overlay-sync"||!oe(a,G))return;const w=a==null?void 0:a.ranks;w&&(le(!!(a!=null&&a.liveActive)),se(we({ranks:w},t.rankKey,10)))},$=async()=>{try{const[d,a,w]=await Promise.all([fetch(J(S,`/snapshot?type=rank-config-sync&board=${t.id}&sourceId=${encodeURIComponent(u)}`),{cache:"no-store"}).then(W=>W.json()).catch(()=>null),fetch(J(S,`/snapshot?type=${t.snapshotType}&board=${t.id}&sourceId=${encodeURIComponent(u)}`),{cache:"no-store"}).then(W=>W.json()).catch(()=>null),fetch(J(S,`/snapshot?type=rank-overlay-sync&board=${G}&sourceId=${encodeURIComponent(u)}`),{cache:"no-store"}).then(W=>W.json()).catch(()=>null)]);if(B)return;const m=I((d==null?void 0:d.payload)??d);(m==null?void 0:m.type)==="rank-config-sync"&&U(m);const j=I((a==null?void 0:a.payload)??a);(j==null?void 0:j.type)===t.snapshotType&&r({...j,updatedAt:Number(j.updatedAt||j.at||Date.now())});const q=I((w==null?void 0:w.payload)??w);(q==null?void 0:q.type)==="rank-overlay-sync"&&M(q)}catch{}},P=[],T=d=>{try{const a=new EventSource(J(S,d));a.onmessage=w=>{try{const m=I(JSON.parse(w.data));if((m==null?void 0:m.type)==="rank-config-sync"){U(m);return}if((m==null?void 0:m.type)===t.snapshotType){r({...m,updatedAt:Number(m.updatedAt||m.at||Date.now())});return}(m==null?void 0:m.type)==="rank-overlay-sync"&&M(m)}catch{}},P.push(a)}catch{}};T(`/events?type=rank-config-sync&board=${encodeURIComponent(t.id)}&sourceId=${encodeURIComponent(u)}`),T(`/events?type=${encodeURIComponent(t.snapshotType)}&board=${encodeURIComponent(t.id)}&sourceId=${encodeURIComponent(u)}`),T(`/events?type=rank-overlay-sync&board=${encodeURIComponent(G)}&sourceId=${encodeURIComponent(u)}`),T("/events");const D=(ge=(Y=(fe=window.liveplay)==null?void 0:fe.app)==null?void 0:Y.onOverlaySync)==null?void 0:ge.call(Y,d=>{const a=I(d);if((a==null?void 0:a.type)==="rank-config-sync"){U(a);return}if((a==null?void 0:a.type)===t.snapshotType){r({...a,updatedAt:Number(a.updatedAt||a.at||Date.now())});return}(a==null?void 0:a.type)==="rank-overlay-sync"&&M(a)}),V=async()=>{var d;try{const a=(d=window.liveplay)==null?void 0:d.app;if(!(a!=null&&a.getState)||!(a!=null&&a.getStatus)){await $();return}const[w,m]=await Promise.all([a.getState(),a.getStatus()]);if(B)return;le(!!(m!=null&&m.tiktokConnected)),se(we(w,t.rankKey,10)),await $()}catch{B||await $()}};V();const Ae=[80,220,500,900,1500,2400].map(d=>window.setTimeout(()=>{B||$()},d)),Fe=window.setInterval(()=>{document.visibilityState==="visible"&&V()},5e3);return()=>{B=!0,Ae.forEach(d=>window.clearTimeout(d)),window.removeEventListener("storage",s),window.removeEventListener(`liveplay:${t.id}-snapshot`,l),window.removeEventListener(`liveplay:${t.id}-config`,b),window.clearInterval(F),window.clearInterval(Fe),x==null||x.close(),P.forEach(d=>d.close()),typeof D=="function"&&D()}},[S,k,t,u]);const de=t.id==="topGift"||t.id==="topCombo"?1:y.topCount,Re=t.id==="weeklyRank"||t.id==="monthlyRank",K=!!(g!=null&&g.visible)&&g.mode==="test"&&((me=g.entries)==null?void 0:me.length),A=K?g.entries.slice(0,de):Se||Re?ke.slice(0,de):[];if(h.useEffect(()=>{if(!K||!g)return;const o=t.id==="topGift"||t.id==="topCombo"?8:Math.max(1,Math.min(60,Number(y.displaySeconds||f.displaySeconds))),r=window.setTimeout(()=>{H(s=>!s||s.mode!=="test"||s.updatedAt!==g.updatedAt?s:{...s,visible:!1,updatedAt:Date.now()})},o*1e3+450);return()=>window.clearTimeout(r)},[K,g==null?void 0:g.updatedAt,t.id,y]),h.useEffect(()=>{const o=Ue(A,K?g==null?void 0:g.updatedAt:0);if(!o){z.current="";return}if(z.current&&z.current!==o){ce(!0);const r=window.setTimeout(()=>ce(!1),950);return z.current=o,()=>window.clearTimeout(r)}z.current=o},[A,K,g==null?void 0:g.updatedAt]),(t.id==="topGift"||t.id==="topCombo")&&A.length){const o=re(t,y),r=A[0],s=Me({username:r.name,nickname:r.name,avatarUrl:r.avatar,giftId:r.giftId,giftName:r.giftName,giftImageUrl:r.giftImageUrl,comboCount:r.comboCount,coinValue:r.coinValue,value:r.value},t.id);return s?p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:"flex-start",padding:0,overflow:"hidden"},children:p.jsx("div",{style:{width:390,maxWidth:"100%",padding:10,boxSizing:"border-box",background:"transparent",border:"none",borderRadius:16,boxShadow:"none"},children:p.jsx(Te,{kind:t.id,config:o,entry:s,embedded:!0})})}):p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}})}if(!A.length)return p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}});const i=y,Ie=i.compactMode?8:10,v=i.compactMode?42:48,pe=i.compactMode?30:34,ue=(i.compactMode?13:15)*(i.nameFontSize/100),Be=(i.compactMode?12:14)*(i.valueFontSize/100),$e=2*(i.lineSpacing/100),C=Ge(i.theme,i.opacity);return p.jsxs(p.Fragment,{children:[p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:i.alignRight?"flex-end":"flex-start",padding:0,overflow:"hidden"},children:p.jsx("div",{style:{width:Math.min(i.width,1400),minHeight:180,borderRadius:16,border:C.shellBorder,background:C.shellBackground,padding:i.compactMode?"8px 10px":"10px 12px",boxSizing:"border-box",overflow:"hidden",boxShadow:C.shadow,isolation:"isolate"},children:p.jsx("div",{style:{display:"grid",gap:Ie,maxWidth:"100%"},children:A.map((o,r)=>{const s=ze(o.medal),l=r===0,b=r===1,x=r===2,F=Z&&(l||b||x),B=`${o.position}:${o.name}:${o.avatar||""}`,G=!!(o.avatar&&!Ce.has(B)),M=p.jsxs("div",{style:{minWidth:0,display:"grid",gap:$e,position:"relative",zIndex:2,justifyItems:i.alignRight?"end":"start"},children:[p.jsx("div",{style:{color:i.theme==="light"&&i.nameColor===f.nameColor?C.defaultNameColor:i.nameColor,fontFamily:ae(i.nameFont),fontWeight:l?900:800,fontSize:l?ue+2:ue,lineHeight:1.1,letterSpacing:`${i.nameLetterSpacing}px`,whiteSpace:"nowrap",overflow:"hidden",textOverflow:"ellipsis",textShadow:l?"0 0 10px rgba(255,242,0,.28)":"none"},children:o.name}),p.jsxs("div",{style:{display:"flex",alignItems:"center",gap:6,justifyContent:i.alignRight?"flex-end":"flex-start",color:i.theme==="light"&&i.valueColor===f.valueColor?C.defaultValueColor:i.valueColor,fontFamily:ae(i.valueFont),fontWeight:800,fontSize:Be,letterSpacing:`${i.valueLetterSpacing}px`},children:[i.showMetricIcon?Ke(t.metricIcon,l,F,i.theme==="light"&&i.valueColor===f.valueColor?C.defaultValueColor:i.valueColor,i.valueFontSize,i.valueFont):null,p.jsx("span",{className:F?"value-bump":"",style:{display:"inline-block",textShadow:l?"0 0 10px rgba(34,211,238,.22)":"none"},children:Le(o.value)})]})]}),$=i.showAvatars?p.jsxs("div",{style:{position:"relative",width:v,height:v,minWidth:v,minHeight:v,maxWidth:v,maxHeight:v,borderRadius:"50%",overflow:"visible",border:l?"2px solid rgba(255,215,0,.95)":"2px solid rgba(255,193,7,.75)",background:C.avatarBackground,display:"grid",placeItems:"center",zIndex:2,boxShadow:l?"0 0 18px rgba(255,210,0,.45)":b||x?"0 0 12px rgba(255,255,255,.12)":"none"},children:[G?p.jsx("img",{src:o.avatar,alt:"",draggable:!1,style:{position:"absolute",inset:0,width:v,height:v,minWidth:v,minHeight:v,maxWidth:v,maxHeight:v,objectFit:"cover",objectPosition:"center",borderRadius:"50%",display:"block",lineHeight:0},referrerPolicy:"no-referrer",onError:()=>{xe(D=>{const V=new Set(D);return V.add(B),V})}}):p.jsx("span",{style:{color:C.avatarInitialColor,fontSize:Math.max(18,v*.42),fontWeight:800},children:o.name.charAt(0).toUpperCase()}),i.showCrown&&o.crowned?p.jsx("span",{className:Z?"crown crown-bounce":"crown",style:{position:"absolute",top:-18,left:8,fontSize:i.compactMode?22:24,zIndex:3,filter:"drop-shadow(0 0 10px rgba(255,200,0,.55))"},children:"👑"}):null]}):null,P=i.showMedals?p.jsx("div",{style:{color:l?"#ffb72e":o.position<=3?"#ffffff":"#ff3434",fontSize:l?24:o.position<=3?20:32,fontWeight:900,lineHeight:1,textAlign:"center",position:"relative",zIndex:2,textShadow:l?"0 0 12px rgba(255,183,46,.65)":"none"},children:s||p.jsxs("span",{style:{display:"inline-block",minWidth:20},children:[o.position,"."]})}):null,T=i.alignRight?`minmax(0, 1fr)${i.showAvatars?` ${v}px`:""}${i.showMedals?` ${pe}px`:""}`:`${i.showMedals?`${pe}px `:""}${i.showAvatars?`${v}px `:""}minmax(0, 1fr)`;return p.jsxs("div",{className:["lp-rank-row",l?"top1":"",b?"top2":"",x?"top3":"",F?"event-pulse":""].join(" ").trim(),style:{display:"grid",gridTemplateColumns:T,alignItems:"center",columnGap:10,minHeight:i.compactMode?46:54,padding:l?"6px 8px":"4px 6px",borderRadius:14,background:l?C.rowBackgroundTop:C.rowBackground,border:i.theme==="light"?"1px solid rgba(15,23,42,.06)":"1px solid rgba(255,255,255,.045)",position:"relative",transform:F&&l?"scale(1.035)":"scale(1)",transition:"transform 220ms ease, filter 220ms ease"},children:[l?p.jsx("div",{className:Z?"top1-halo pulse":"top1-halo",style:{position:"absolute",inset:"2px 2px 2px 2px",borderRadius:16,pointerEvents:"none",background:C.topHalo,filter:"blur(8px)",opacity:.95}}):null,i.alignRight?p.jsxs(p.Fragment,{children:[M,$,P]}):p.jsxs(p.Fragment,{children:[P,$,M]})]},`${o.position}-${o.name}-${o.value}`)})})})}),p.jsx("style",{children:`
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
      `})]})}export{Ye as RanksOverlayPage};
