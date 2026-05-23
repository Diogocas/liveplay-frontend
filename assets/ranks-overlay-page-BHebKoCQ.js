import{r as y,j as p}from"./index-QjyvAncm.js";import{n as $e,c as Fe,d as J}from"./top-record-widget-VCbf9Rt0.js";import{g as he}from"./app-kv-DDdMRHVm.js";import{r as Me}from"./overlay-url-IdlNOR_r.js";import"./brazil-gifts-DLMOKDcl.js";const Te="http://127.0.0.1:35942",j={topLike:{id:"topLike",metricIcon:"heart",configKey:"liveplay.toplike.config",snapshotKey:"liveplay.toplike.snapshot",channel:"liveplay-toplike-channel",snapshotType:"top-like-sync",rankKey:"likes",queryBoard:"topLike"},topGifts:{id:"topGifts",metricIcon:"coin",configKey:"liveplay.topgifts.config",snapshotKey:"liveplay.topgifts.snapshot",channel:"liveplay-topgifts-channel",snapshotType:"top-gifts-sync",rankKey:"gifts",queryBoard:"topGifts"},weeklyRank:{id:"weeklyRank",metricIcon:"coin",configKey:"liveplay.weeklyrank.config",snapshotKey:"liveplay.weeklyrank.snapshot",channel:"liveplay-weeklyrank-channel",snapshotType:"weekly-rank-sync",rankKey:"weekly",queryBoard:"weeklyRank"},monthlyRank:{id:"monthlyRank",metricIcon:"coin",configKey:"liveplay.monthlyrank.config",snapshotKey:"liveplay.monthlyrank.snapshot",channel:"liveplay-monthlyrank-channel",snapshotType:"monthly-rank-sync",rankKey:"monthly",queryBoard:"monthlyRank"},topGift:{id:"topGift",metricIcon:"coin",configKey:"liveplay.topgift.config",snapshotKey:"liveplay.topgift.snapshot",channel:"liveplay-topgift-channel",snapshotType:"top-gift-sync",rankKey:"topGift",queryBoard:"topGift"},topCombo:{id:"topCombo",metricIcon:"coin",configKey:"liveplay.topcombo.config",snapshotKey:"liveplay.topcombo.snapshot",channel:"liveplay-topcombo-channel",snapshotType:"top-combo-sync",rankKey:"topCombo",queryBoard:"topCombo"}};function je(n){return n.trim().replace(/\/$/,"")}function D(n,e){return`${je(n)}${e.startsWith("/")?e:`/${e}`}`}const m={theme:"dark",opacity:100,topCount:10,showAvatars:!0,showMedals:!0,showCrown:!0,showMetricIcon:!0,displaySeconds:8,width:820,compactMode:!1,nameFont:"display",valueFont:"display",nameColor:"#fff200",valueColor:"#22d3ee",nameFontSize:100,valueFontSize:100,nameLetterSpacing:0,valueLetterSpacing:0,lineSpacing:100,alignRight:!1},ye={display:'"Trebuchet MS", "Arial Black", Impact, sans-serif',default:'Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',tech:'"Lucida Console", "Courier New", monospace',compact:'"Arial Narrow", "Roboto Condensed", "Segoe UI", sans-serif',serif:'Georgia, "Times New Roman", serif'};function te(n){return ye[n]||ye.default}function be(n){const e=typeof n=="string"?JSON.parse(n):n??{};return{...m,...e,topCount:e.topCount===3||e.topCount===5||e.topCount===10?e.topCount:m.topCount,theme:e.theme==="light"?"light":"dark",opacity:typeof e.opacity=="number"?Math.max(0,Math.min(100,e.opacity)):m.opacity,displaySeconds:typeof e.displaySeconds=="number"?Math.max(1,Math.min(60,e.displaySeconds)):m.displaySeconds,width:typeof e.width=="number"?Math.max(420,Math.min(1400,e.width)):m.width,nameFont:e.nameFont==="default"||e.nameFont==="display"||e.nameFont==="tech"||e.nameFont==="compact"||e.nameFont==="serif"?e.nameFont:m.nameFont,valueFont:e.valueFont==="default"||e.valueFont==="display"||e.valueFont==="tech"||e.valueFont==="compact"||e.valueFont==="serif"?e.valueFont:m.valueFont,nameColor:typeof e.nameColor=="string"&&e.nameColor.trim()?e.nameColor:m.nameColor,valueColor:typeof e.valueColor=="string"&&e.valueColor.trim()?e.valueColor:m.valueColor,nameFontSize:typeof e.nameFontSize=="number"?Math.max(70,Math.min(180,e.nameFontSize)):m.nameFontSize,valueFontSize:typeof e.valueFontSize=="number"?Math.max(70,Math.min(180,e.valueFontSize)):m.valueFontSize,nameLetterSpacing:typeof e.nameLetterSpacing=="number"?Math.max(-2,Math.min(12,e.nameLetterSpacing)):m.nameLetterSpacing,valueLetterSpacing:typeof e.valueLetterSpacing=="number"?Math.max(-2,Math.min(12,e.valueLetterSpacing)):m.valueLetterSpacing,lineSpacing:typeof e.lineSpacing=="number"?Math.max(70,Math.min(180,e.lineSpacing)):m.lineSpacing,alignRight:typeof e.alignRight=="boolean"?e.alignRight:m.alignRight}}function Y(n){try{const e=window.localStorage.getItem(n.configKey);return e?be(e):m}catch{return m}}function oe(n,e){const u=typeof e=="string"?JSON.parse(e):e??{};return{...J(n.id),...u}}function q(n){try{const e=window.localStorage.getItem(n.configKey);return e?oe(n,e):J(n.id)}catch{return J(n.id)}}function ee(n){try{const e=window.localStorage.getItem(n.snapshotKey);if(!e)return null;const u=JSON.parse(e);return u.type!==n.snapshotType?null:u}catch{return null}}function ve(n){return n.id==="topLike"?"likes":n.id==="topGifts"?"gifts":n.id==="weeklyRank"?"weeklyCoins":n.id==="monthlyRank"?"monthlyCoins":n.id==="topGift"?"topGift":"topCombo"}function Ee(n){return n==="gold"?"🥇":n==="silver"?"🥈":n==="bronze"?"🥉":null}function Ne(n){return new Intl.NumberFormat("pt-BR").format(n)}function we(n,e,u){var b,k;const t=e==="weekly"?"weeklyCoins":e==="monthly"?"monthlyCoins":e;return(Array.isArray((k=(b=n==null?void 0:n.ranks)==null?void 0:b[t])==null?void 0:k.entries)?n.ranks[t].entries:[]).slice(0,u).map((c,h)=>({position:h+1,name:String((c==null?void 0:c.nickname)||(c==null?void 0:c.username)||`Usuário ${h+1}`),value:Math.max(0,Number((c==null?void 0:c.value)||0)),avatar:String((c==null?void 0:c.avatarUrl)||"").trim()||void 0,medal:h===0?"gold":h===1?"silver":h===2?"bronze":void 0,crowned:h===0,giftId:String((c==null?void 0:c.giftId)||"").trim()||void 0,giftName:String((c==null?void 0:c.giftName)||"").trim()||void 0,giftImageUrl:String((c==null?void 0:c.giftImageUrl)||"").trim()||void 0,comboCount:Math.max(0,Number((c==null?void 0:c.comboCount)||0))||void 0,coinValue:Math.max(0,Number((c==null?void 0:c.coinValue)||0))||void 0}))}function ze(n,e){return n.map(u=>`${u.position}:${u.name}:${u.value}`).join("|")+`::${e||0}`}function Le(n,e,u,t,x=100,b="display"){return n==="coin"?p.jsx("span",{className:u?"metric-icon metric-coin event-coin":"metric-icon metric-coin",style:{color:t,fontSize:(e?17:15)*(x/100),lineHeight:1,fontFamily:te(b),filter:e?"drop-shadow(0 0 8px rgba(255,207,63,.45))":"none"},children:"●"}):p.jsx("span",{className:u?"metric-icon metric-heart event-heart":"metric-icon metric-heart",style:{color:"#ff2c55",fontSize:e?17:15,lineHeight:1,filter:e?"drop-shadow(0 0 8px rgba(255,44,85,.45))":"none"},children:"♥"})}function E(n,e){const u=Math.max(0,Math.min(1,Number(n)/100));return Math.max(0,Math.min(e,u*e))}function Ue(n,e){const u=E(e,n==="light"?.98:.88),t=E(e,n==="light"?.96:.1),x=E(e,n==="light"?.72:.035),b=E(e,.08),k=E(e,n==="light"?.16:.14),c=E(e,n==="light"?.18:.28);return n==="light"?{shellBackground:`rgba(248,250,252,${u})`,shellBorder:`1px solid rgba(15,23,42,${k})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${t}), rgba(226,232,240,${x}))`,rowBackground:`rgba(255,255,255,${x})`,avatarBackground:`rgba(15,23,42,${b})`,avatarInitialColor:"#0f172a",defaultNameColor:"#0f172a",defaultValueColor:"#0369a1",topHalo:"radial-gradient(circle at 12% 50%, rgba(59,130,246,.22), transparent 24%), radial-gradient(circle at 72% 50%, rgba(250,204,21,.22), transparent 42%)",shadow:c>0?`0 18px 36px rgba(15,23,42,${c})`:"none"}:{shellBackground:`rgba(4,7,18,${u})`,shellBorder:`1px solid rgba(255,255,255,${k})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${t}), rgba(255,255,255,${x}))`,rowBackground:`rgba(255,255,255,${x})`,avatarBackground:`rgba(255,255,255,${b})`,avatarInitialColor:"#d1d5db",defaultNameColor:"#fff200",defaultValueColor:"#22d3ee",topHalo:"radial-gradient(circle at 12% 50%, rgba(255,80,120,.35), transparent 24%), radial-gradient(circle at 72% 50%, rgba(255,195,0,.18), transparent 42%)",shadow:c>0?`0 18px 40px rgba(0,0,0,${c})`:"none"}}function He(){var de;const n=y.useMemo(()=>new URLSearchParams(window.location.search),[]),e=n.get("board")||"topLike",u=n.get("sourceId")||e,t=e==="topGifts"?j.topGifts:e==="weeklyRank"?j.weeklyRank:e==="monthlyRank"?j.monthlyRank:e==="topGift"?j.topGift:e==="topCombo"?j.topCombo:j.topLike,x=n.get("bridgeUrl"),b=typeof window<"u"&&typeof window.liveplay<"u",[k,c]=y.useState(()=>x||Me(n,"/overlay-bridge",Te)),[h,ne]=y.useState(()=>t.id==="topGift"||t.id==="topCombo"?q(t):Y(t)),[g,P]=y.useState(()=>ee(t)),[ke,ae]=y.useState([]),[Ce,re]=y.useState(!1),[_,ie]=y.useState(!1),V=y.useRef((g==null?void 0:g.updatedAt)??0),N=y.useRef(""),Q=y.useRef(Number((h==null?void 0:h.updatedAt)||0)),X=y.useRef(!!(h!=null&&h.updatedAt)),z=o=>{if(!o){X.current||ne(t.id==="topGift"||t.id==="topCombo"?J(t.id):m);return}const r=t.id==="topGift"||t.id==="topCombo"?oe(t,o):be(o),s=Number((r==null?void 0:r.updatedAt)||0);if(s>0){if(s<Q.current)return;Q.current=s,X.current=!0}else if(X.current)return;ne(r)},R=o=>o!=null&&o.payload&&typeof o.payload=="object"?o.payload:o,Z=(o,r)=>{const s=String((o==null?void 0:o.board)||"").trim(),l=String((o==null?void 0:o.sourceId)||"").trim(),w=new Set([t.id,t.queryBoard,ve(t),r||"",u].filter(Boolean));return!(s&&!w.has(s)||l&&!w.has(l))},L=o=>{var l,w;const r=R(o);if(!r||r.type!=="rank-config-sync"||!Z(r))return;const s=Number(r.updatedAt||((l=r.config)==null?void 0:l.updatedAt)||0);s&&s<Q.current||z({...r.config,updatedAt:s||Number(((w=r.config)==null?void 0:w.updatedAt)||Date.now())})};y.useEffect(()=>{const o=document.documentElement,r=document.body,s=document.getElementById("root"),l=o.style.background,w=r.style.background,S=(s==null?void 0:s.style.background)??"";return o.style.background="transparent",r.style.background="transparent",s&&(s.style.background="transparent"),()=>{o.style.background=l,r.style.background=w,s&&(s.style.background=S)}},[]),y.useEffect(()=>{var o,r,s;b&&((s=(r=(o=window.liveplay)==null?void 0:o.app)==null?void 0:r.getOverlayBridgeUrl)==null||s.call(r).then(l=>{l!=null&&l.url&&c(l.url)}).catch(()=>{}))},[b]),y.useEffect(()=>{z(t.id==="topGift"||t.id==="topCombo"?q(t):Y(t));const o=ee(t);P(o),V.current=(o==null?void 0:o.updatedAt)??0},[t]),y.useEffect(()=>{let o=!0;return(async()=>{const[s,l]=await Promise.all([he(t.configKey,null),he(t.snapshotKey,null)]);o&&(s&&z(s),l&&l.type===t.snapshotType&&(!l.board||l.board===t.id)&&(P(l),V.current=l.updatedAt??0))})().catch(()=>{}),()=>{o=!1}},[t]),y.useEffect(()=>{var me,O,ge;const o=()=>z(t.id==="topGift"||t.id==="topCombo"?q(t):Y(t)),r=d=>{const a=R(d);if(!a||a.type!==t.snapshotType||!Z(a))return;d=a;const v=Number(d.updatedAt||d.at||Date.now());v<V.current||(V.current=v,P({...d,updatedAt:v}))},s=d=>{d.key===t.configKey&&o(),d.key===t.snapshotKey&&r(ee(t))},l=d=>{r(d.detail)},w=d=>{L(d.detail)};window.addEventListener("storage",s),window.addEventListener(`liveplay:${t.id}-snapshot`,l),window.addEventListener(`liveplay:${t.id}-config`,w);let S=null;try{S=new BroadcastChannel(t.channel),S.onmessage=d=>{const a=R(d.data);if((a==null?void 0:a.type)==="rank-config-sync"){L(a);return}r(a)}}catch{}const A=window.setInterval(()=>{document.visibilityState==="visible"&&z(t.id==="topGift"||t.id==="topCombo"?q(t):Y(t))},2500);let I=!1;const $=ve(t),F=d=>{const a=R(d);if(!a||(a==null?void 0:a.type)!=="rank-overlay-sync"||!Z(a,$))return;const v=a==null?void 0:a.ranks;v&&(re(!!(a!=null&&a.liveActive)),ae(we({ranks:v},t.rankKey,10)))},M=async()=>{try{const[d,a,v]=await Promise.all([fetch(D(k,`/snapshot?type=rank-config-sync&board=${t.id}&sourceId=${encodeURIComponent(u)}`),{cache:"no-store"}).then(K=>K.json()).catch(()=>null),fetch(D(k,`/snapshot?type=${t.snapshotType}&board=${t.id}&sourceId=${encodeURIComponent(u)}`),{cache:"no-store"}).then(K=>K.json()).catch(()=>null),fetch(D(k,`/snapshot?type=rank-overlay-sync&board=${$}&sourceId=${encodeURIComponent(u)}`),{cache:"no-store"}).then(K=>K.json()).catch(()=>null)]);if(I)return;const f=R((d==null?void 0:d.payload)??d);(f==null?void 0:f.type)==="rank-config-sync"&&L(f);const T=R((a==null?void 0:a.payload)??a);(T==null?void 0:T.type)===t.snapshotType&&r({...T,updatedAt:Number(T.updatedAt||T.at||Date.now())});const H=R((v==null?void 0:v.payload)??v);(H==null?void 0:H.type)==="rank-overlay-sync"&&F(H)}catch{}},pe=[],W=d=>{try{const a=new EventSource(D(k,d));a.onmessage=v=>{try{const f=R(JSON.parse(v.data));if((f==null?void 0:f.type)==="rank-config-sync"){L(f);return}if((f==null?void 0:f.type)===t.snapshotType){r({...f,updatedAt:Number(f.updatedAt||f.at||Date.now())});return}(f==null?void 0:f.type)==="rank-overlay-sync"&&F(f)}catch{}},pe.push(a)}catch{}};W(`/events?type=rank-config-sync&board=${encodeURIComponent(t.id)}&sourceId=${encodeURIComponent(u)}`),W(`/events?type=${encodeURIComponent(t.snapshotType)}&board=${encodeURIComponent(t.id)}&sourceId=${encodeURIComponent(u)}`),W(`/events?type=rank-overlay-sync&board=${encodeURIComponent($)}&sourceId=${encodeURIComponent(u)}`),W("/events");const ue=(ge=(O=(me=window.liveplay)==null?void 0:me.app)==null?void 0:O.onOverlaySync)==null?void 0:ge.call(O,d=>{const a=R(d);if((a==null?void 0:a.type)==="rank-config-sync"){L(a);return}if((a==null?void 0:a.type)===t.snapshotType){r({...a,updatedAt:Number(a.updatedAt||a.at||Date.now())});return}(a==null?void 0:a.type)==="rank-overlay-sync"&&F(a)}),fe=async()=>{var d;try{const a=(d=window.liveplay)==null?void 0:d.app;if(!(a!=null&&a.getState)||!(a!=null&&a.getStatus)){await M();return}const[v,f]=await Promise.all([a.getState(),a.getStatus()]);if(I)return;re(!!(f!=null&&f.tiktokConnected)),ae(we(v,t.rankKey,10)),await M()}catch{I||await M()}};fe();const Be=[80,220,500,900,1500,2400].map(d=>window.setTimeout(()=>{I||M()},d)),Ae=window.setInterval(()=>{document.visibilityState==="visible"&&fe()},5e3);return()=>{I=!0,Be.forEach(d=>window.clearTimeout(d)),window.removeEventListener("storage",s),window.removeEventListener(`liveplay:${t.id}-snapshot`,l),window.removeEventListener(`liveplay:${t.id}-config`,w),window.clearInterval(A),window.clearInterval(Ae),S==null||S.close(),pe.forEach(d=>d.close()),typeof ue=="function"&&ue()}},[k,b,t,u]);const se=t.id==="topGift"||t.id==="topCombo"?1:h.topCount,Se=t.id==="weeklyRank"||t.id==="monthlyRank",U=!!(g!=null&&g.visible)&&g.mode==="test"&&((de=g.entries)==null?void 0:de.length),B=U?g.entries.slice(0,se):Ce||Se?ke.slice(0,se):[];if(y.useEffect(()=>{if(!U||!g)return;const o=t.id==="topGift"||t.id==="topCombo"?8:Math.max(1,Math.min(60,Number(h.displaySeconds||m.displaySeconds))),r=window.setTimeout(()=>{P(s=>!s||s.mode!=="test"||s.updatedAt!==g.updatedAt?s:{...s,visible:!1,updatedAt:Date.now()})},o*1e3+450);return()=>window.clearTimeout(r)},[U,g==null?void 0:g.updatedAt,t.id,h]),y.useEffect(()=>{const o=ze(B,U?g==null?void 0:g.updatedAt:0);if(!o){N.current="";return}if(N.current&&N.current!==o){ie(!0);const r=window.setTimeout(()=>ie(!1),950);return N.current=o,()=>window.clearTimeout(r)}N.current=o},[B,U,g==null?void 0:g.updatedAt]),(t.id==="topGift"||t.id==="topCombo")&&B.length){const o=oe(t,h),r=B[0],s=$e({username:r.name,nickname:r.name,avatarUrl:r.avatar,giftId:r.giftId,giftName:r.giftName,giftImageUrl:r.giftImageUrl,comboCount:r.comboCount,coinValue:r.coinValue,value:r.value},t.id);return s?p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:"flex-start",padding:0,overflow:"hidden"},children:p.jsx("div",{style:{width:390,maxWidth:"100%",padding:10,boxSizing:"border-box",background:"transparent",border:"none",borderRadius:16,boxShadow:"none"},children:p.jsx(Fe,{kind:t.id,config:o,entry:s,embedded:!0})})}):p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}})}if(!B.length)return p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}});const i=h,xe=i.compactMode?8:10,G=i.compactMode?42:48,le=i.compactMode?30:34,ce=(i.compactMode?13:15)*(i.nameFontSize/100),Re=(i.compactMode?12:14)*(i.valueFontSize/100),Ie=2*(i.lineSpacing/100),C=Ue(i.theme,i.opacity);return p.jsxs(p.Fragment,{children:[p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:i.alignRight?"flex-end":"flex-start",padding:0,overflow:"hidden"},children:p.jsx("div",{style:{width:Math.min(i.width,1400),minHeight:180,borderRadius:16,border:C.shellBorder,background:C.shellBackground,padding:i.compactMode?"8px 10px":"10px 12px",boxSizing:"border-box",overflow:"hidden",boxShadow:C.shadow,isolation:"isolate"},children:p.jsx("div",{style:{display:"grid",gap:xe,maxWidth:"100%"},children:B.map((o,r)=>{const s=Ee(o.medal),l=r===0,w=r===1,S=r===2,A=_&&(l||w||S),I=p.jsxs("div",{style:{minWidth:0,display:"grid",gap:Ie,position:"relative",zIndex:2,justifyItems:i.alignRight?"end":"start"},children:[p.jsx("div",{style:{color:i.theme==="light"&&i.nameColor===m.nameColor?C.defaultNameColor:i.nameColor,fontFamily:te(i.nameFont),fontWeight:l?900:800,fontSize:l?ce+2:ce,lineHeight:1.1,letterSpacing:`${i.nameLetterSpacing}px`,whiteSpace:"nowrap",overflow:"hidden",textOverflow:"ellipsis",textShadow:l?"0 0 10px rgba(255,242,0,.28)":"none"},children:o.name}),p.jsxs("div",{style:{display:"flex",alignItems:"center",gap:6,justifyContent:i.alignRight?"flex-end":"flex-start",color:i.theme==="light"&&i.valueColor===m.valueColor?C.defaultValueColor:i.valueColor,fontFamily:te(i.valueFont),fontWeight:800,fontSize:Re,letterSpacing:`${i.valueLetterSpacing}px`},children:[i.showMetricIcon?Le(t.metricIcon,l,A,i.theme==="light"&&i.valueColor===m.valueColor?C.defaultValueColor:i.valueColor,i.valueFontSize,i.valueFont):null,p.jsx("span",{className:A?"value-bump":"",style:{display:"inline-block",textShadow:l?"0 0 10px rgba(34,211,238,.22)":"none"},children:Ne(o.value)})]})]}),$=i.showAvatars?p.jsxs("div",{style:{position:"relative",width:G,height:G,borderRadius:"50%",overflow:"visible",border:l?"2px solid rgba(255,215,0,.95)":"2px solid rgba(255,193,7,.75)",background:C.avatarBackground,display:"grid",placeItems:"center",zIndex:2,boxShadow:l?"0 0 18px rgba(255,210,0,.45)":w||S?"0 0 12px rgba(255,255,255,.12)":"none"},children:[o.avatar?p.jsx("img",{src:o.avatar,alt:o.name,style:{width:"100%",height:"100%",objectFit:"cover",borderRadius:"50%"},referrerPolicy:"no-referrer"}):p.jsx("span",{style:{color:C.avatarInitialColor,fontSize:Math.max(18,G*.42),fontWeight:800},children:o.name.charAt(0).toUpperCase()}),i.showCrown&&o.crowned?p.jsx("span",{className:_?"crown crown-bounce":"crown",style:{position:"absolute",top:-18,left:8,fontSize:i.compactMode?22:24,zIndex:3,filter:"drop-shadow(0 0 10px rgba(255,200,0,.55))"},children:"👑"}):null]}):null,F=i.showMedals?p.jsx("div",{style:{color:l?"#ffb72e":o.position<=3?"#ffffff":"#ff3434",fontSize:l?24:o.position<=3?20:32,fontWeight:900,lineHeight:1,textAlign:"center",position:"relative",zIndex:2,textShadow:l?"0 0 12px rgba(255,183,46,.65)":"none"},children:s||p.jsxs("span",{style:{display:"inline-block",minWidth:20},children:[o.position,"."]})}):null,M=i.alignRight?`minmax(0, 1fr)${i.showAvatars?` ${G}px`:""}${i.showMedals?` ${le}px`:""}`:`${i.showMedals?`${le}px `:""}${i.showAvatars?`${G}px `:""}minmax(0, 1fr)`;return p.jsxs("div",{className:["lp-rank-row",l?"top1":"",w?"top2":"",S?"top3":"",A?"event-pulse":""].join(" ").trim(),style:{display:"grid",gridTemplateColumns:M,alignItems:"center",columnGap:10,minHeight:i.compactMode?46:54,padding:l?"6px 8px":"4px 6px",borderRadius:14,background:l?C.rowBackgroundTop:C.rowBackground,border:i.theme==="light"?"1px solid rgba(15,23,42,.06)":"1px solid rgba(255,255,255,.045)",position:"relative",transform:A&&l?"scale(1.035)":"scale(1)",transition:"transform 220ms ease, filter 220ms ease"},children:[l?p.jsx("div",{className:_?"top1-halo pulse":"top1-halo",style:{position:"absolute",inset:"2px 2px 2px 2px",borderRadius:16,pointerEvents:"none",background:C.topHalo,filter:"blur(8px)",opacity:.95}}):null,i.alignRight?p.jsxs(p.Fragment,{children:[I,$,F]}):p.jsxs(p.Fragment,{children:[F,$,I]})]},`${o.position}-${o.name}-${o.value}`)})})})}),p.jsx("style",{children:`
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
      `})]})}export{He as RanksOverlayPage};
