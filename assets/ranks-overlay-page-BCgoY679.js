import{r as v,j as u,f as ve}from"./index-D8mGcNSq.js";import{n as Ue,c as je,d as ee}from"./top-record-widget-BfsNVuyl.js";import{r as Te}from"./overlay-url-yJYaczpp.js";import"./brazil-gifts-CAp08RMG.js";const Ee="http://127.0.0.1:35942",z={topLike:{id:"topLike",metricIcon:"heart",configKey:"liveplay.toplike.config",snapshotKey:"liveplay.toplike.snapshot",channel:"liveplay-toplike-channel",snapshotType:"top-like-sync",rankKey:"likes",queryBoard:"topLike"},topGifts:{id:"topGifts",metricIcon:"coin",configKey:"liveplay.topgifts.config",snapshotKey:"liveplay.topgifts.snapshot",channel:"liveplay-topgifts-channel",snapshotType:"top-gifts-sync",rankKey:"gifts",queryBoard:"topGifts"},weeklyRank:{id:"weeklyRank",metricIcon:"coin",configKey:"liveplay.weeklyrank.config",snapshotKey:"liveplay.weeklyrank.snapshot",channel:"liveplay-weeklyrank-channel",snapshotType:"weekly-rank-sync",rankKey:"weekly",queryBoard:"weeklyRank"},monthlyRank:{id:"monthlyRank",metricIcon:"coin",configKey:"liveplay.monthlyrank.config",snapshotKey:"liveplay.monthlyrank.snapshot",channel:"liveplay-monthlyrank-channel",snapshotType:"monthly-rank-sync",rankKey:"monthly",queryBoard:"monthlyRank"},topGift:{id:"topGift",metricIcon:"coin",configKey:"liveplay.topgift.config",snapshotKey:"liveplay.topgift.snapshot",channel:"liveplay-topgift-channel",snapshotType:"top-gift-sync",rankKey:"topGift",queryBoard:"topGift"},topCombo:{id:"topCombo",metricIcon:"coin",configKey:"liveplay.topcombo.config",snapshotKey:"liveplay.topcombo.snapshot",channel:"liveplay-topcombo-channel",snapshotType:"top-combo-sync",rankKey:"topCombo",queryBoard:"topCombo"}};function Ne(e){return e.trim().replace(/\/$/,"")}function U(e,t){return`${Ne(e)}${t.startsWith("/")?t:`/${t}`}`}const g={theme:"dark",opacity:100,topCount:10,showAvatars:!0,showMedals:!0,showCrown:!0,showMetricIcon:!0,displaySeconds:8,width:820,compactMode:!1,nameFont:"display",valueFont:"display",nameColor:"#fff200",valueColor:"#22d3ee",nameFontSize:100,valueFontSize:100,nameLetterSpacing:0,valueLetterSpacing:0,lineSpacing:100,alignRight:!1},be={display:'"Trebuchet MS", "Arial Black", Impact, sans-serif',default:'Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',tech:'"Lucida Console", "Courier New", monospace',compact:'"Arial Narrow", "Roboto Condensed", "Segoe UI", sans-serif',serif:'Georgia, "Times New Roman", serif'};function re(e){return be[e]||be.default}function ke(e){const t=typeof e=="string"?JSON.parse(e):e??{};return{...g,...t,topCount:t.topCount===3||t.topCount===5||t.topCount===10?t.topCount:g.topCount,theme:t.theme==="light"?"light":"dark",opacity:typeof t.opacity=="number"?Math.max(0,Math.min(100,t.opacity)):g.opacity,displaySeconds:typeof t.displaySeconds=="number"?Math.max(1,Math.min(60,t.displaySeconds)):g.displaySeconds,width:typeof t.width=="number"?Math.max(420,Math.min(1400,t.width)):g.width,nameFont:t.nameFont==="default"||t.nameFont==="display"||t.nameFont==="tech"||t.nameFont==="compact"||t.nameFont==="serif"?t.nameFont:g.nameFont,valueFont:t.valueFont==="default"||t.valueFont==="display"||t.valueFont==="tech"||t.valueFont==="compact"||t.valueFont==="serif"?t.valueFont:g.valueFont,nameColor:typeof t.nameColor=="string"&&t.nameColor.trim()?t.nameColor:g.nameColor,valueColor:typeof t.valueColor=="string"&&t.valueColor.trim()?t.valueColor:g.valueColor,nameFontSize:typeof t.nameFontSize=="number"?Math.max(70,Math.min(180,t.nameFontSize)):g.nameFontSize,valueFontSize:typeof t.valueFontSize=="number"?Math.max(70,Math.min(180,t.valueFontSize)):g.valueFontSize,nameLetterSpacing:typeof t.nameLetterSpacing=="number"?Math.max(-2,Math.min(12,t.nameLetterSpacing)):g.nameLetterSpacing,valueLetterSpacing:typeof t.valueLetterSpacing=="number"?Math.max(-2,Math.min(12,t.valueLetterSpacing)):g.valueLetterSpacing,lineSpacing:typeof t.lineSpacing=="number"?Math.max(70,Math.min(180,t.lineSpacing)):g.lineSpacing,alignRight:typeof t.alignRight=="boolean"?t.alignRight:g.alignRight}}function X(e){try{const t=window.localStorage.getItem(e.configKey);return t?ke(t):g}catch{return g}}function se(e,t){const d=typeof t=="string"?JSON.parse(t):t??{};return{...ee(e.id),...d}}function Z(e){try{const t=window.localStorage.getItem(e.configKey);return t?se(e,t):ee(e.id)}catch{return ee(e.id)}}function ie(e){try{const t=window.localStorage.getItem(e.snapshotKey);if(!t)return null;const d=JSON.parse(t);return d.type!==e.snapshotType?null:d}catch{return null}}function we(e){return e.id==="topLike"?"likes":e.id==="topGifts"?"gifts":e.id==="weeklyRank"?"weeklyCoins":e.id==="monthlyRank"?"monthlyCoins":e.id==="topGift"?"topGift":"topCombo"}function ze(e){return e==="gold"?"🥇":e==="silver"?"🥈":e==="bronze"?"🥉":null}function Le(e){return new Intl.NumberFormat("pt-BR").format(e)}function Ge(e){var d,o,R,C,b;const t=[e==null?void 0:e.avatar,e==null?void 0:e.avatarUrl,e==null?void 0:e.profilePictureUrl,e==null?void 0:e.profilePicture,e==null?void 0:e.photoUrl,e==null?void 0:e.pictureUrl,e==null?void 0:e.imageUrl,(d=e==null?void 0:e.user)==null?void 0:d.avatar,(o=e==null?void 0:e.user)==null?void 0:o.avatarUrl,(R=e==null?void 0:e.user)==null?void 0:R.profilePictureUrl,(C=e==null?void 0:e.user)==null?void 0:C.profilePicture,(b=e==null?void 0:e.user)==null?void 0:b.photoUrl];for(const r of t){const h=String(r||"").trim();if(h)return h}}function ye(e,t,d){var C,b;const o=t==="weekly"?"weeklyCoins":t==="monthly"?"monthlyCoins":t;return(Array.isArray((b=(C=e==null?void 0:e.ranks)==null?void 0:C[o])==null?void 0:b.entries)?e.ranks[o].entries:[]).slice(0,d).map((r,h)=>{var G,f;return{position:h+1,name:String((r==null?void 0:r.nickname)||(r==null?void 0:r.username)||(r==null?void 0:r.name)||((G=r==null?void 0:r.user)==null?void 0:G.nickname)||((f=r==null?void 0:r.user)==null?void 0:f.username)||`Usuário ${h+1}`),value:Math.max(0,Number((r==null?void 0:r.value)||0)),avatar:Ge(r),medal:h===0?"gold":h===1?"silver":h===2?"bronze":void 0,crowned:h===0,giftId:String((r==null?void 0:r.giftId)||"").trim()||void 0,giftName:String((r==null?void 0:r.giftName)||"").trim()||void 0,giftImageUrl:String((r==null?void 0:r.giftImageUrl)||"").trim()||void 0,comboCount:Math.max(0,Number((r==null?void 0:r.comboCount)||0))||void 0,coinValue:Math.max(0,Number((r==null?void 0:r.coinValue)||0))||void 0}})}function Ke(e,t){return e.map(d=>`${d.position}:${d.name}:${d.value}`).join("|")+`::${t||0}`}function Pe(e,t,d,o,R=100,C="display"){return e==="coin"?u.jsx("span",{className:d?"metric-icon metric-coin event-coin":"metric-icon metric-coin",style:{color:o,fontSize:(t?17:15)*(R/100),lineHeight:1,fontFamily:re(C),filter:t?"drop-shadow(0 0 8px rgba(255,207,63,.45))":"none"},children:"●"}):u.jsx("span",{className:d?"metric-icon metric-heart event-heart":"metric-icon metric-heart",style:{color:"#ff2c55",fontSize:t?17:15,lineHeight:1,filter:t?"drop-shadow(0 0 8px rgba(255,44,85,.45))":"none"},children:"♥"})}function L(e,t){const d=Math.max(0,Math.min(1,Number(e)/100));return Math.max(0,Math.min(t,d*t))}function Ve(e,t){const d=L(t,e==="light"?.98:.88),o=L(t,e==="light"?.96:.1),R=L(t,e==="light"?.72:.035),C=L(t,.08),b=L(t,e==="light"?.16:.14),r=L(t,e==="light"?.18:.28);return e==="light"?{shellBackground:`rgba(248,250,252,${d})`,shellBorder:`1px solid rgba(15,23,42,${b})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${o}), rgba(226,232,240,${R}))`,rowBackground:`rgba(255,255,255,${R})`,avatarBackground:`rgba(15,23,42,${C})`,avatarInitialColor:"#0f172a",defaultNameColor:"#0f172a",defaultValueColor:"#0369a1",topHalo:"radial-gradient(circle at 12% 50%, rgba(59,130,246,.22), transparent 24%), radial-gradient(circle at 72% 50%, rgba(250,204,21,.22), transparent 42%)",shadow:r>0?`0 18px 36px rgba(15,23,42,${r})`:"none"}:{shellBackground:`rgba(4,7,18,${d})`,shellBorder:`1px solid rgba(255,255,255,${b})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${o}), rgba(255,255,255,${R}))`,rowBackground:`rgba(255,255,255,${R})`,avatarBackground:`rgba(255,255,255,${C})`,avatarInitialColor:"#d1d5db",defaultNameColor:"#fff200",defaultValueColor:"#22d3ee",topHalo:"radial-gradient(circle at 12% 50%, rgba(255,80,120,.35), transparent 24%), radial-gradient(circle at 72% 50%, rgba(255,195,0,.18), transparent 42%)",shadow:r>0?`0 18px 40px rgba(0,0,0,${r})`:"none"}}function Ye(){var me;const e=v.useMemo(()=>new URLSearchParams(window.location.search),[]),t=e.get("board")||"topLike",d=e.get("sourceId")||t,o=t==="topGifts"?z.topGifts:t==="weeklyRank"?z.weeklyRank:t==="monthlyRank"?z.monthlyRank:t==="topGift"?z.topGift:t==="topCombo"?z.topCombo:z.topLike,R=e.get("bridgeUrl"),C=typeof window<"u"&&typeof window.liveplay<"u",[b,r]=v.useState(()=>R||Te(e,"/overlay-bridge",Ee)),[h,G]=v.useState(()=>o.id==="topGift"||o.id==="topCombo"?Z(o):X(o)),[f,D]=v.useState(()=>ie(o)),[Ce,le]=v.useState([]),[Se,ce]=v.useState(!1),[te,de]=v.useState(!1),[xe,Re]=v.useState(()=>new Set),q=v.useRef((f==null?void 0:f.updatedAt)??0),K=v.useRef(""),oe=v.useRef(Number((h==null?void 0:h.updatedAt)||0)),ne=v.useRef(!!(h!=null&&h.updatedAt)),P=n=>{if(!n){ne.current||G(o.id==="topGift"||o.id==="topCombo"?ee(o.id):g);return}const i=o.id==="topGift"||o.id==="topCombo"?se(o,n):ke(n),l=Number((i==null?void 0:i.updatedAt)||0);if(l>0){if(l<oe.current)return;oe.current=l,ne.current=!0}else if(ne.current)return;G(i)},B=n=>n!=null&&n.payload&&typeof n.payload=="object"?n.payload:n,ae=(n,i)=>{const l=String((n==null?void 0:n.board)||"").trim(),c=String((n==null?void 0:n.sourceId)||"").trim(),S=new Set([o.id,o.queryBoard,we(o),i||"",d].filter(Boolean));return!(l&&!S.has(l)||c&&!S.has(c))},V=n=>{var c,S;const i=B(n);if(!i||i.type!=="rank-config-sync"||!ae(i))return;const l=Number(i.updatedAt||((c=i.config)==null?void 0:c.updatedAt)||0);l&&l<oe.current||P({...i.config,updatedAt:l||Number(((S=i.config)==null?void 0:S.updatedAt)||Date.now())})};v.useEffect(()=>{const n=document.documentElement,i=document.body,l=document.getElementById("root"),c=n.style.background,S=i.style.background,$=(l==null?void 0:l.style.background)??"";return n.style.background="transparent",i.style.background="transparent",l&&(l.style.background="transparent"),()=>{n.style.background=c,i.style.background=S,l&&(l.style.background=$)}},[]),v.useEffect(()=>{var n,i,l;C&&((l=(i=(n=window.liveplay)==null?void 0:n.app)==null?void 0:i.getOverlayBridgeUrl)==null||l.call(i).then(c=>{c!=null&&c.url&&r(c.url)}).catch(()=>{}))},[C]),v.useEffect(()=>{P(o.id==="topGift"||o.id==="topCombo"?Z(o):X(o));const n=ie(o);D(n),q.current=(n==null?void 0:n.updatedAt)??0},[o]),v.useEffect(()=>{let n=!0;return(async()=>{const[l,c]=await Promise.all([ve(o.configKey,null),ve(o.snapshotKey,null)]);n&&(l&&P(l),c&&c.type===o.snapshotType&&(!c.board||c.board===o.id)&&(D(c),q.current=c.updatedAt??0))})().catch(()=>{}),()=>{n=!1}},[o]),v.useEffect(()=>{var ge,J,he;const n=()=>P(o.id==="topGift"||o.id==="topCombo"?Z(o):X(o)),i=p=>{const a=B(p);if(!a||a.type!==o.snapshotType||!ae(a))return;p=a;const k=Number(p.updatedAt||p.at||Date.now());k<q.current||(q.current=k,D({...p,updatedAt:k}))},l=p=>{p.key===o.configKey&&n(),p.key===o.snapshotKey&&i(ie(o))},c=p=>{i(p.detail)},S=p=>{V(p.detail)};window.addEventListener("storage",l),window.addEventListener(`liveplay:${o.id}-snapshot`,c),window.addEventListener(`liveplay:${o.id}-config`,S);let $=null;try{$=new BroadcastChannel(o.channel),$.onmessage=p=>{const a=B(p.data);if((a==null?void 0:a.type)==="rank-config-sync"){V(a);return}i(a)}}catch{}const T=window.setInterval(()=>{document.visibilityState==="visible"&&P(o.id==="topGift"||o.id==="topCombo"?Z(o):X(o))},2500);let F=!1;const x=we(o),E=p=>{const a=B(p);if(!a||(a==null?void 0:a.type)!=="rank-overlay-sync"||!ae(a,x))return;const k=a==null?void 0:a.ranks;k&&(ce(!!(a!=null&&a.liveActive)),le(ye({ranks:k},o.rankKey,10)))},M=async()=>{try{const p=[fetch(U(b,`/snapshot?type=rank-config-sync&board=${o.id}&sourceId=${encodeURIComponent(d)}`),{cache:"no-store"}).then(w=>w.json()).catch(()=>null)];(o.id==="topLike"||o.id==="topGifts")&&p.push(fetch(U(b,`/snapshot?type=rank-config-sync&board=${x}&sourceId=${encodeURIComponent(d)}`),{cache:"no-store"}).then(w=>w.json()).catch(()=>null),fetch(U(b,`/snapshot?type=rank-config-sync&board=${x}&sourceId=${encodeURIComponent(x)}`),{cache:"no-store"}).then(w=>w.json()).catch(()=>null),fetch(U(b,`/snapshot?type=rank-config-sync&board=${o.id}&sourceId=${encodeURIComponent(x)}`),{cache:"no-store"}).then(w=>w.json()).catch(()=>null));const[a,k,m]=await Promise.all([Promise.all(p),fetch(U(b,`/snapshot?type=${o.snapshotType}&board=${o.id}&sourceId=${encodeURIComponent(d)}`),{cache:"no-store"}).then(w=>w.json()).catch(()=>null),fetch(U(b,`/snapshot?type=rank-overlay-sync&board=${x}&sourceId=${encodeURIComponent(d)}`),{cache:"no-store"}).then(w=>w.json()).catch(()=>null)]);if(F)return;for(const w of a){const Q=B((w==null?void 0:w.payload)??w);(Q==null?void 0:Q.type)==="rank-config-sync"&&V(Q)}const N=B((k==null?void 0:k.payload)??k);(N==null?void 0:N.type)===o.snapshotType&&i({...N,updatedAt:Number(N.updatedAt||N.at||Date.now())});const _=B((m==null?void 0:m.payload)??m);(_==null?void 0:_.type)==="rank-overlay-sync"&&E(_)}catch{}},H=[],A=p=>{try{const a=new EventSource(U(b,p));a.onmessage=k=>{try{const m=B(JSON.parse(k.data));if((m==null?void 0:m.type)==="rank-config-sync"){V(m);return}if((m==null?void 0:m.type)===o.snapshotType){i({...m,updatedAt:Number(m.updatedAt||m.at||Date.now())});return}(m==null?void 0:m.type)==="rank-overlay-sync"&&E(m)}catch{}},H.push(a)}catch{}};A(`/events?type=rank-config-sync&board=${encodeURIComponent(o.id)}&sourceId=${encodeURIComponent(d)}`),(o.id==="topLike"||o.id==="topGifts")&&(A(`/events?type=rank-config-sync&board=${encodeURIComponent(x)}&sourceId=${encodeURIComponent(d)}`),A(`/events?type=rank-config-sync&board=${encodeURIComponent(x)}&sourceId=${encodeURIComponent(x)}`),A(`/events?type=rank-config-sync&board=${encodeURIComponent(o.id)}&sourceId=${encodeURIComponent(x)}`)),A(`/events?type=${encodeURIComponent(o.snapshotType)}&board=${encodeURIComponent(o.id)}&sourceId=${encodeURIComponent(d)}`),A(`/events?type=rank-overlay-sync&board=${encodeURIComponent(x)}&sourceId=${encodeURIComponent(d)}`),A("/events");const Y=(he=(J=(ge=window.liveplay)==null?void 0:ge.app)==null?void 0:J.onOverlaySync)==null?void 0:he.call(J,p=>{const a=B(p);if((a==null?void 0:a.type)==="rank-config-sync"){V(a);return}if((a==null?void 0:a.type)===o.snapshotType){i({...a,updatedAt:Number(a.updatedAt||a.at||Date.now())});return}(a==null?void 0:a.type)==="rank-overlay-sync"&&E(a)}),O=async()=>{var p;try{const a=(p=window.liveplay)==null?void 0:p.app;if(!(a!=null&&a.getState)||!(a!=null&&a.getStatus)){await M();return}const[k,m]=await Promise.all([a.getState(),a.getStatus()]);if(F)return;ce(!!(m!=null&&m.tiktokConnected)),le(ye(k,o.rankKey,10)),await M()}catch{F||await M()}};O();const Fe=[80,220,500,900,1500,2400].map(p=>window.setTimeout(()=>{F||M()},p)),Me=window.setInterval(()=>{document.visibilityState==="visible"&&O()},5e3);return()=>{F=!0,Fe.forEach(p=>window.clearTimeout(p)),window.removeEventListener("storage",l),window.removeEventListener(`liveplay:${o.id}-snapshot`,c),window.removeEventListener(`liveplay:${o.id}-config`,S),window.clearInterval(T),window.clearInterval(Me),$==null||$.close(),H.forEach(p=>p.close()),typeof Y=="function"&&Y()}},[b,C,o,d]);const pe=o.id==="topGift"||o.id==="topCombo"?1:h.topCount,Ie=o.id==="weeklyRank"||o.id==="monthlyRank",W=!!(f!=null&&f.visible)&&f.mode==="test"&&((me=f.entries)==null?void 0:me.length),j=W?f.entries.slice(0,pe):Se||Ie?Ce.slice(0,pe):[];if(v.useEffect(()=>{if(!W||!f)return;const n=o.id==="topGift"||o.id==="topCombo"?8:Math.max(1,Math.min(60,Number(h.displaySeconds||g.displaySeconds))),i=window.setTimeout(()=>{D(l=>!l||l.mode!=="test"||l.updatedAt!==f.updatedAt?l:{...l,visible:!1,updatedAt:Date.now()})},n*1e3+450);return()=>window.clearTimeout(i)},[W,f==null?void 0:f.updatedAt,o.id,h]),v.useEffect(()=>{const n=Ke(j,W?f==null?void 0:f.updatedAt:0);if(!n){K.current="";return}if(K.current&&K.current!==n){de(!0);const i=window.setTimeout(()=>de(!1),950);return K.current=n,()=>window.clearTimeout(i)}K.current=n},[j,W,f==null?void 0:f.updatedAt]),(o.id==="topGift"||o.id==="topCombo")&&j.length){const n=se(o,h),i=j[0],l=Ue({username:i.name,nickname:i.name,avatarUrl:i.avatar,giftId:i.giftId,giftName:i.giftName,giftImageUrl:i.giftImageUrl,comboCount:i.comboCount,coinValue:i.coinValue,value:i.value},o.id);return l?u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:"flex-start",padding:0,overflow:"hidden"},children:u.jsx("div",{style:{width:390,maxWidth:"100%",padding:10,boxSizing:"border-box",background:"transparent",border:"none",borderRadius:16,boxShadow:"none"},children:u.jsx(je,{kind:o.id,config:n,entry:l,embedded:!0})})}):u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}})}if(!j.length)return u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}});const s=h,$e=s.compactMode?8:10,y=s.compactMode?42:48,ue=s.compactMode?30:34,fe=(s.compactMode?13:15)*(s.nameFontSize/100),Be=(s.compactMode?12:14)*(s.valueFontSize/100),Ae=2*(s.lineSpacing/100),I=Ve(s.theme,s.opacity);return u.jsxs(u.Fragment,{children:[u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:s.alignRight?"flex-end":"flex-start",padding:0,overflow:"hidden"},children:u.jsx("div",{style:{width:Math.min(s.width,1400),minHeight:180,borderRadius:16,border:I.shellBorder,background:I.shellBackground,padding:s.compactMode?"8px 10px":"10px 12px",boxSizing:"border-box",overflow:"hidden",boxShadow:I.shadow,isolation:"isolate"},children:u.jsx("div",{style:{display:"grid",gap:$e,maxWidth:"100%"},children:j.map((n,i)=>{const l=ze(n.medal),c=i===0,S=i===1,$=i===2,T=te&&(c||S||$),F=`${n.position}:${n.name}:${n.avatar||""}`,x=!!(n.avatar&&!xe.has(F)),E=u.jsxs("div",{style:{minWidth:0,display:"grid",gap:Ae,position:"relative",zIndex:2,justifyItems:s.alignRight?"end":"start"},children:[u.jsx("div",{style:{color:s.theme==="light"&&s.nameColor===g.nameColor?I.defaultNameColor:s.nameColor,fontFamily:re(s.nameFont),fontWeight:c?900:800,fontSize:c?fe+2:fe,lineHeight:1.1,letterSpacing:`${s.nameLetterSpacing}px`,whiteSpace:"nowrap",overflow:"hidden",textOverflow:"ellipsis",textShadow:c?"0 0 10px rgba(255,242,0,.28)":"none"},children:n.name}),u.jsxs("div",{style:{display:"flex",alignItems:"center",gap:6,justifyContent:s.alignRight?"flex-end":"flex-start",color:s.theme==="light"&&s.valueColor===g.valueColor?I.defaultValueColor:s.valueColor,fontFamily:re(s.valueFont),fontWeight:800,fontSize:Be,letterSpacing:`${s.valueLetterSpacing}px`},children:[s.showMetricIcon?Pe(o.metricIcon,c,T,s.theme==="light"&&s.valueColor===g.valueColor?I.defaultValueColor:s.valueColor,s.valueFontSize,s.valueFont):null,u.jsx("span",{className:T?"value-bump":"",style:{display:"inline-block",textShadow:c?"0 0 10px rgba(34,211,238,.22)":"none"},children:Le(n.value)})]})]}),M=s.showAvatars?u.jsxs("div",{style:{position:"relative",width:y,height:y,minWidth:y,minHeight:y,maxWidth:y,maxHeight:y,borderRadius:"50%",overflow:"visible",border:c?"2px solid rgba(255,215,0,.95)":"2px solid rgba(255,193,7,.75)",background:I.avatarBackground,display:"grid",placeItems:"center",zIndex:2,boxShadow:c?"0 0 18px rgba(255,210,0,.45)":S||$?"0 0 12px rgba(255,255,255,.12)":"none"},children:[x?u.jsx("img",{src:n.avatar,alt:"",draggable:!1,style:{position:"absolute",inset:0,width:y,height:y,minWidth:y,minHeight:y,maxWidth:y,maxHeight:y,objectFit:"cover",objectPosition:"center",borderRadius:"50%",display:"block",lineHeight:0},referrerPolicy:"no-referrer",onError:()=>{Re(Y=>{const O=new Set(Y);return O.add(F),O})}}):u.jsx("span",{style:{color:I.avatarInitialColor,fontSize:Math.max(18,y*.42),fontWeight:800},children:n.name.charAt(0).toUpperCase()}),s.showCrown&&n.crowned?u.jsx("span",{className:te?"crown crown-bounce":"crown",style:{position:"absolute",top:-18,left:8,fontSize:s.compactMode?22:24,zIndex:3,filter:"drop-shadow(0 0 10px rgba(255,200,0,.55))"},children:"👑"}):null]}):null,H=s.showMedals?u.jsx("div",{style:{color:c?"#ffb72e":n.position<=3?"#ffffff":"#ff3434",fontSize:c?24:n.position<=3?20:32,fontWeight:900,lineHeight:1,textAlign:"center",position:"relative",zIndex:2,textShadow:c?"0 0 12px rgba(255,183,46,.65)":"none"},children:l||u.jsxs("span",{style:{display:"inline-block",minWidth:20},children:[n.position,"."]})}):null,A=s.alignRight?`minmax(0, 1fr)${s.showAvatars?` ${y}px`:""}${s.showMedals?` ${ue}px`:""}`:`${s.showMedals?`${ue}px `:""}${s.showAvatars?`${y}px `:""}minmax(0, 1fr)`;return u.jsxs("div",{className:["lp-rank-row",c?"top1":"",S?"top2":"",$?"top3":"",T?"event-pulse":""].join(" ").trim(),style:{display:"grid",gridTemplateColumns:A,alignItems:"center",columnGap:10,minHeight:s.compactMode?46:54,padding:c?"6px 8px":"4px 6px",borderRadius:14,background:c?I.rowBackgroundTop:I.rowBackground,border:s.theme==="light"?"1px solid rgba(15,23,42,.06)":"1px solid rgba(255,255,255,.045)",position:"relative",transform:T&&c?"scale(1.035)":"scale(1)",transition:"transform 220ms ease, filter 220ms ease"},children:[c?u.jsx("div",{className:te?"top1-halo pulse":"top1-halo",style:{position:"absolute",inset:"2px 2px 2px 2px",borderRadius:16,pointerEvents:"none",background:I.topHalo,filter:"blur(8px)",opacity:.95}}):null,s.alignRight?u.jsxs(u.Fragment,{children:[E,M,H]}):u.jsxs(u.Fragment,{children:[H,M,E]})]},`${n.position}-${n.name}-${n.value}`)})})})}),u.jsx("style",{children:`
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
