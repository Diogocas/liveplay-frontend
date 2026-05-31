import{r as v,j as u,f as we}from"./index-CUOfYOTV.js";import{n as je,c as Ee,d as te}from"./top-record-widget-BH3WAPpu.js";import{r as Ne}from"./overlay-url-yJYaczpp.js";import"./brazil-gifts-CAp08RMG.js";const Ue="http://127.0.0.1:35942",z={topLike:{id:"topLike",metricIcon:"heart",configKey:"liveplay.toplike.config",snapshotKey:"liveplay.toplike.snapshot",channel:"liveplay-toplike-channel",snapshotType:"top-like-sync",rankKey:"likes",queryBoard:"topLike"},topGifts:{id:"topGifts",metricIcon:"coin",configKey:"liveplay.topgifts.config",snapshotKey:"liveplay.topgifts.snapshot",channel:"liveplay-topgifts-channel",snapshotType:"top-gifts-sync",rankKey:"gifts",queryBoard:"topGifts"},weeklyRank:{id:"weeklyRank",metricIcon:"coin",configKey:"liveplay.weeklyrank.config",snapshotKey:"liveplay.weeklyrank.snapshot",channel:"liveplay-weeklyrank-channel",snapshotType:"weekly-rank-sync",rankKey:"weekly",queryBoard:"weeklyRank"},monthlyRank:{id:"monthlyRank",metricIcon:"coin",configKey:"liveplay.monthlyrank.config",snapshotKey:"liveplay.monthlyrank.snapshot",channel:"liveplay-monthlyrank-channel",snapshotType:"monthly-rank-sync",rankKey:"monthly",queryBoard:"monthlyRank"},topGift:{id:"topGift",metricIcon:"coin",configKey:"liveplay.topgift.config",snapshotKey:"liveplay.topgift.snapshot",channel:"liveplay-topgift-channel",snapshotType:"top-gift-sync",rankKey:"topGift",queryBoard:"topGift"},topCombo:{id:"topCombo",metricIcon:"coin",configKey:"liveplay.topcombo.config",snapshotKey:"liveplay.topcombo.snapshot",channel:"liveplay-topcombo-channel",snapshotType:"top-combo-sync",rankKey:"topCombo",queryBoard:"topCombo"}};function ze(e){return e.trim().replace(/\/$/,"")}function X(e,t){return`${ze(e)}${t.startsWith("/")?t:`/${t}`}`}const g={theme:"dark",opacity:100,topCount:10,showAvatars:!0,showMedals:!0,showCrown:!0,showMetricIcon:!0,displaySeconds:8,width:820,compactMode:!1,nameFont:"display",valueFont:"display",nameColor:"#fff200",valueColor:"#22d3ee",nameFontSize:100,valueFontSize:100,nameLetterSpacing:0,valueLetterSpacing:0,lineSpacing:100,alignRight:!1},be={display:'"Trebuchet MS", "Arial Black", Impact, sans-serif',default:'Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',tech:'"Lucida Console", "Courier New", monospace',compact:'"Arial Narrow", "Roboto Condensed", "Segoe UI", sans-serif',serif:'Georgia, "Times New Roman", serif'};function se(e){return be[e]||be.default}function Se(e){const t=typeof e=="string"?JSON.parse(e):e??{};return{...g,...t,topCount:t.topCount===3||t.topCount===5||t.topCount===10?t.topCount:g.topCount,theme:t.theme==="light"?"light":"dark",opacity:typeof t.opacity=="number"?Math.max(0,Math.min(100,t.opacity)):g.opacity,displaySeconds:typeof t.displaySeconds=="number"?Math.max(1,Math.min(60,t.displaySeconds)):g.displaySeconds,width:typeof t.width=="number"?Math.max(420,Math.min(1400,t.width)):g.width,nameFont:t.nameFont==="default"||t.nameFont==="display"||t.nameFont==="tech"||t.nameFont==="compact"||t.nameFont==="serif"?t.nameFont:g.nameFont,valueFont:t.valueFont==="default"||t.valueFont==="display"||t.valueFont==="tech"||t.valueFont==="compact"||t.valueFont==="serif"?t.valueFont:g.valueFont,nameColor:typeof t.nameColor=="string"&&t.nameColor.trim()?t.nameColor:g.nameColor,valueColor:typeof t.valueColor=="string"&&t.valueColor.trim()?t.valueColor:g.valueColor,nameFontSize:typeof t.nameFontSize=="number"?Math.max(70,Math.min(180,t.nameFontSize)):g.nameFontSize,valueFontSize:typeof t.valueFontSize=="number"?Math.max(70,Math.min(180,t.valueFontSize)):g.valueFontSize,nameLetterSpacing:typeof t.nameLetterSpacing=="number"?Math.max(-2,Math.min(12,t.nameLetterSpacing)):g.nameLetterSpacing,valueLetterSpacing:typeof t.valueLetterSpacing=="number"?Math.max(-2,Math.min(12,t.valueLetterSpacing)):g.valueLetterSpacing,lineSpacing:typeof t.lineSpacing=="number"?Math.max(70,Math.min(180,t.lineSpacing)):g.lineSpacing,alignRight:typeof t.alignRight=="boolean"?t.alignRight:g.alignRight}}function Z(e){try{const t=window.localStorage.getItem(e.configKey);return t?Se(t):g}catch{return g}}function le(e,t){const d=typeof t=="string"?JSON.parse(t):t??{};return{...te(e.id),...d}}function ee(e){try{const t=window.localStorage.getItem(e.configKey);return t?le(e,t):te(e.id)}catch{return te(e.id)}}function re(e){try{const t=window.localStorage.getItem(e.snapshotKey);if(!t)return null;const d=JSON.parse(t);return d.type!==e.snapshotType?null:d}catch{return null}}function ye(e){return e.id==="topLike"?"likes":e.id==="topGifts"?"gifts":e.id==="weeklyRank"?"weeklyCoins":e.id==="monthlyRank"?"monthlyCoins":e.id==="topGift"?"topGift":"topCombo"}function Le(e){return e==="gold"?"🥇":e==="silver"?"🥈":e==="bronze"?"🥉":null}function Ke(e){return new Intl.NumberFormat("pt-BR").format(e)}function Ge(e){var d,o,S,b,y;const t=[e==null?void 0:e.avatar,e==null?void 0:e.avatarUrl,e==null?void 0:e.profilePictureUrl,e==null?void 0:e.profilePicture,e==null?void 0:e.photoUrl,e==null?void 0:e.pictureUrl,e==null?void 0:e.imageUrl,(d=e==null?void 0:e.user)==null?void 0:d.avatar,(o=e==null?void 0:e.user)==null?void 0:o.avatarUrl,(S=e==null?void 0:e.user)==null?void 0:S.profilePictureUrl,(b=e==null?void 0:e.user)==null?void 0:b.profilePicture,(y=e==null?void 0:e.user)==null?void 0:y.photoUrl];for(const r of t){const h=String(r||"").trim();if(h)return h}}function ke(e,t,d){var b,y;const o=t==="weekly"?"weeklyCoins":t==="monthly"?"monthlyCoins":t;return(Array.isArray((y=(b=e==null?void 0:e.ranks)==null?void 0:b[o])==null?void 0:y.entries)?e.ranks[o].entries:[]).slice(0,d).map((r,h)=>{var K,f;return{position:h+1,name:String((r==null?void 0:r.nickname)||(r==null?void 0:r.username)||(r==null?void 0:r.name)||((K=r==null?void 0:r.user)==null?void 0:K.nickname)||((f=r==null?void 0:r.user)==null?void 0:f.username)||`Usuário ${h+1}`),value:Math.max(0,Number((r==null?void 0:r.value)||0)),avatar:Ge(r),medal:h===0?"gold":h===1?"silver":h===2?"bronze":void 0,crowned:h===0,giftId:String((r==null?void 0:r.giftId)||"").trim()||void 0,giftName:String((r==null?void 0:r.giftName)||"").trim()||void 0,giftImageUrl:String((r==null?void 0:r.giftImageUrl)||"").trim()||void 0,comboCount:Math.max(0,Number((r==null?void 0:r.comboCount)||0))||void 0,coinValue:Math.max(0,Number((r==null?void 0:r.coinValue)||0))||void 0}})}function Pe(e,t){return e.map(d=>`${d.position}:${d.name}:${d.value}`).join("|")+`::${t||0}`}function Ve(e,t,d,o,S=100,b="display"){return e==="coin"?u.jsx("span",{className:d?"metric-icon metric-coin event-coin":"metric-icon metric-coin",style:{color:o,fontSize:(t?17:15)*(S/100),lineHeight:1,fontFamily:se(b),filter:t?"drop-shadow(0 0 8px rgba(255,207,63,.45))":"none"},children:"●"}):u.jsx("span",{className:d?"metric-icon metric-heart event-heart":"metric-icon metric-heart",style:{color:"#ff2c55",fontSize:t?17:15,lineHeight:1,filter:t?"drop-shadow(0 0 8px rgba(255,44,85,.45))":"none"},children:"♥"})}function L(e,t){const d=Math.max(0,Math.min(1,Number(e)/100));return Math.max(0,Math.min(t,d*t))}function We(e,t){const d=L(t,e==="light"?.98:.88),o=L(t,e==="light"?.96:.1),S=L(t,e==="light"?.72:.035),b=L(t,.08),y=L(t,e==="light"?.16:.14),r=L(t,e==="light"?.18:.28);return e==="light"?{shellBackground:`rgba(248,250,252,${d})`,shellBorder:`1px solid rgba(15,23,42,${y})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${o}), rgba(226,232,240,${S}))`,rowBackground:`rgba(255,255,255,${S})`,avatarBackground:`rgba(15,23,42,${b})`,avatarInitialColor:"#0f172a",defaultNameColor:"#0f172a",defaultValueColor:"#0369a1",topHalo:"radial-gradient(circle at 12% 50%, rgba(59,130,246,.22), transparent 24%), radial-gradient(circle at 72% 50%, rgba(250,204,21,.22), transparent 42%)",shadow:r>0?`0 18px 36px rgba(15,23,42,${r})`:"none"}:{shellBackground:`rgba(4,7,18,${d})`,shellBorder:`1px solid rgba(255,255,255,${y})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${o}), rgba(255,255,255,${S}))`,rowBackground:`rgba(255,255,255,${S})`,avatarBackground:`rgba(255,255,255,${b})`,avatarInitialColor:"#d1d5db",defaultNameColor:"#fff200",defaultValueColor:"#22d3ee",topHalo:"radial-gradient(circle at 12% 50%, rgba(255,80,120,.35), transparent 24%), radial-gradient(circle at 72% 50%, rgba(255,195,0,.18), transparent 42%)",shadow:r>0?`0 18px 40px rgba(0,0,0,${r})`:"none"}}function Je(){var ge;const e=v.useMemo(()=>new URLSearchParams(window.location.search),[]),t=e.get("board")||"topLike",d=e.get("sourceId")||t,o=t==="topGifts"?z.topGifts:t==="weeklyRank"?z.weeklyRank:t==="monthlyRank"?z.monthlyRank:t==="topGift"?z.topGift:t==="topCombo"?z.topCombo:z.topLike,S=e.get("bridgeUrl"),b=typeof window<"u"&&typeof window.liveplay<"u",[y,r]=v.useState(()=>S||Ne(e,"/overlay-bridge",Ue)),[h,K]=v.useState(()=>o.id==="topGift"||o.id==="topCombo"?ee(o):Z(o)),[f,q]=v.useState(()=>re(o)),[Ce,ce]=v.useState([]),[xe,de]=v.useState(!1),[oe,pe]=v.useState(!1),[Re,Ie]=v.useState(()=>new Set),D=v.useRef((f==null?void 0:f.updatedAt)??0),G=v.useRef(""),ne=v.useRef(Number((h==null?void 0:h.updatedAt)||0)),ae=v.useRef(!!(h!=null&&h.updatedAt)),P=n=>{if(!n){ae.current||K(o.id==="topGift"||o.id==="topCombo"?te(o.id):g);return}const i=o.id==="topGift"||o.id==="topCombo"?le(o,n):Se(n),l=Number((i==null?void 0:i.updatedAt)||0);if(l>0){if(l<ne.current)return;ne.current=l,ae.current=!0}else if(ae.current)return;K(i)},B=n=>n!=null&&n.payload&&typeof n.payload=="object"?n.payload:n,ie=(n,i)=>{const l=String((n==null?void 0:n.board)||"").trim(),c=String((n==null?void 0:n.sourceId)||"").trim(),k=new Set([o.id,o.queryBoard,ye(o),i||"",d].filter(Boolean));return!(l&&!k.has(l)||c&&!k.has(c))},V=n=>{var c,k;const i=B(n);if(!i||i.type!=="rank-config-sync"||!ie(i))return;const l=Number(i.updatedAt||((c=i.config)==null?void 0:c.updatedAt)||0);l&&l<ne.current||P({...i.config,updatedAt:l||Number(((k=i.config)==null?void 0:k.updatedAt)||Date.now())})};v.useEffect(()=>{const n=document.documentElement,i=document.body,l=document.getElementById("root"),c=n.style.background,k=i.style.background,R=(l==null?void 0:l.style.background)??"";return n.style.background="transparent",i.style.background="transparent",l&&(l.style.background="transparent"),()=>{n.style.background=c,i.style.background=k,l&&(l.style.background=R)}},[]),v.useEffect(()=>{var n,i,l;b&&((l=(i=(n=window.liveplay)==null?void 0:n.app)==null?void 0:i.getOverlayBridgeUrl)==null||l.call(i).then(c=>{c!=null&&c.url&&r(c.url)}).catch(()=>{}))},[b]),v.useEffect(()=>{P(o.id==="topGift"||o.id==="topCombo"?ee(o):Z(o));const n=re(o);q(n),D.current=(n==null?void 0:n.updatedAt)??0},[o]),v.useEffect(()=>{let n=!0;return(async()=>{const[l,c]=await Promise.all([we(o.configKey,null),we(o.snapshotKey,null)]);n&&(l&&P(l),c&&c.type===o.snapshotType&&(!c.board||c.board===o.id)&&(q(c),D.current=c.updatedAt??0))})().catch(()=>{}),()=>{n=!1}},[o]),v.useEffect(()=>{var he,J,ve;const n=()=>P(o.id==="topGift"||o.id==="topCombo"?ee(o):Z(o)),i=p=>{const a=B(p);if(!a||a.type!==o.snapshotType||!ie(a))return;p=a;const C=Number(p.updatedAt||p.at||Date.now());C<D.current||(D.current=C,q({...p,updatedAt:C}))},l=p=>{p.key===o.configKey&&n(),p.key===o.snapshotKey&&i(re(o))},c=p=>{i(p.detail)},k=p=>{V(p.detail)};window.addEventListener("storage",l),window.addEventListener(`liveplay:${o.id}-snapshot`,c),window.addEventListener(`liveplay:${o.id}-config`,k);let R=null;try{R=new BroadcastChannel(o.channel),R.onmessage=p=>{const a=B(p.data);if((a==null?void 0:a.type)==="rank-config-sync"){V(a);return}i(a)}}catch{}const M=window.setInterval(()=>{document.visibilityState==="visible"&&P(o.id==="topGift"||o.id==="topCombo"?ee(o):Z(o))},2500);let A=!1;const T=ye(o),j=p=>{const a=B(p);if(!a||(a==null?void 0:a.type)!=="rank-overlay-sync"||!ie(a,T))return;const C=a==null?void 0:a.ranks;C&&(de(!!(a!=null&&a.liveActive)),ce(ke({ranks:C},o.rankKey,10)))},$=async()=>{try{const a=Array.from(new Set([o.id,o.queryBoard,T,d].filter(Boolean))).map(I=>fetch(X(y,`/snapshot?type=rank-config-sync&board=${encodeURIComponent(I)}&sourceId=${encodeURIComponent(d)}`),{cache:"no-store"}).then(U=>U.json()).catch(()=>null)),[C,m,_]=await Promise.all([Promise.all(a),fetch(X(y,`/snapshot?type=${o.snapshotType}&board=${o.id}&sourceId=${encodeURIComponent(d)}`),{cache:"no-store"}).then(I=>I.json()).catch(()=>null),fetch(X(y,`/snapshot?type=rank-overlay-sync&board=${T}&sourceId=${encodeURIComponent(d)}`),{cache:"no-store"}).then(I=>I.json()).catch(()=>null)]);if(A)return;for(const I of C){const U=B((I==null?void 0:I.payload)??I);(U==null?void 0:U.type)==="rank-config-sync"&&V(U)}const N=B((m==null?void 0:m.payload)??m);(N==null?void 0:N.type)===o.snapshotType&&i({...N,updatedAt:Number(N.updatedAt||N.at||Date.now())});const Q=B((_==null?void 0:_.payload)??_);(Q==null?void 0:Q.type)==="rank-overlay-sync"&&j(Q)}catch{}},H=[],E=p=>{try{const a=new EventSource(X(y,p));a.onmessage=C=>{try{const m=B(JSON.parse(C.data));if((m==null?void 0:m.type)==="rank-config-sync"){V(m);return}if((m==null?void 0:m.type)===o.snapshotType){i({...m,updatedAt:Number(m.updatedAt||m.at||Date.now())});return}(m==null?void 0:m.type)==="rank-overlay-sync"&&j(m)}catch{}},H.push(a)}catch{}};E(`/events?type=rank-config-sync&board=${encodeURIComponent(o.id)}&sourceId=${encodeURIComponent(d)}`),E(`/events?type=${encodeURIComponent(o.snapshotType)}&board=${encodeURIComponent(o.id)}&sourceId=${encodeURIComponent(d)}`),E(`/events?type=rank-overlay-sync&board=${encodeURIComponent(T)}&sourceId=${encodeURIComponent(d)}`),E("/events");const Y=(ve=(J=(he=window.liveplay)==null?void 0:he.app)==null?void 0:J.onOverlaySync)==null?void 0:ve.call(J,p=>{const a=B(p);if((a==null?void 0:a.type)==="rank-config-sync"){V(a);return}if((a==null?void 0:a.type)===o.snapshotType){i({...a,updatedAt:Number(a.updatedAt||a.at||Date.now())});return}(a==null?void 0:a.type)==="rank-overlay-sync"&&j(a)}),O=async()=>{var p;try{const a=(p=window.liveplay)==null?void 0:p.app;if(!(a!=null&&a.getState)||!(a!=null&&a.getStatus)){await $();return}const[C,m]=await Promise.all([a.getState(),a.getStatus()]);if(A)return;de(!!(m!=null&&m.tiktokConnected)),ce(ke(C,o.rankKey,10)),await $()}catch{A||await $()}};O();const Me=[80,220,500,900,1500,2400].map(p=>window.setTimeout(()=>{A||$()},p)),Te=window.setInterval(()=>{document.visibilityState==="visible"&&O()},5e3);return()=>{A=!0,Me.forEach(p=>window.clearTimeout(p)),window.removeEventListener("storage",l),window.removeEventListener(`liveplay:${o.id}-snapshot`,c),window.removeEventListener(`liveplay:${o.id}-config`,k),window.clearInterval(M),window.clearInterval(Te),R==null||R.close(),H.forEach(p=>p.close()),typeof Y=="function"&&Y()}},[y,b,o,d]);const ue=o.id==="topGift"||o.id==="topCombo"?1:h.topCount,Be=o.id==="weeklyRank"||o.id==="monthlyRank",W=!!(f!=null&&f.visible)&&f.mode==="test"&&((ge=f.entries)==null?void 0:ge.length),F=W?f.entries.slice(0,ue):xe||Be?Ce.slice(0,ue):[];if(v.useEffect(()=>{if(!W||!f)return;const n=o.id==="topGift"||o.id==="topCombo"?8:Math.max(1,Math.min(60,Number(h.displaySeconds||g.displaySeconds))),i=window.setTimeout(()=>{q(l=>!l||l.mode!=="test"||l.updatedAt!==f.updatedAt?l:{...l,visible:!1,updatedAt:Date.now()})},n*1e3+450);return()=>window.clearTimeout(i)},[W,f==null?void 0:f.updatedAt,o.id,h]),v.useEffect(()=>{const n=Pe(F,W?f==null?void 0:f.updatedAt:0);if(!n){G.current="";return}if(G.current&&G.current!==n){pe(!0);const i=window.setTimeout(()=>pe(!1),950);return G.current=n,()=>window.clearTimeout(i)}G.current=n},[F,W,f==null?void 0:f.updatedAt]),(o.id==="topGift"||o.id==="topCombo")&&F.length){const n=le(o,h),i=F[0],l=je({username:i.name,nickname:i.name,avatarUrl:i.avatar,giftId:i.giftId,giftName:i.giftName,giftImageUrl:i.giftImageUrl,comboCount:i.comboCount,coinValue:i.coinValue,value:i.value},o.id);return l?u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:"flex-start",padding:0,overflow:"hidden"},children:u.jsx("div",{style:{width:390,maxWidth:"100%",padding:10,boxSizing:"border-box",background:"transparent",border:"none",borderRadius:16,boxShadow:"none"},children:u.jsx(Ee,{kind:o.id,config:n,entry:l,embedded:!0})})}):u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}})}if(!F.length)return u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}});const s=h,Ae=s.compactMode?8:10,w=s.compactMode?42:48,fe=s.compactMode?30:34,me=(s.compactMode?13:15)*(s.nameFontSize/100),$e=(s.compactMode?12:14)*(s.valueFontSize/100),Fe=2*(s.lineSpacing/100),x=We(s.theme,s.opacity);return u.jsxs(u.Fragment,{children:[u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:s.alignRight?"flex-end":"flex-start",padding:0,overflow:"hidden"},children:u.jsx("div",{style:{width:Math.min(s.width,1400),minHeight:180,borderRadius:16,border:x.shellBorder,background:x.shellBackground,padding:s.compactMode?"8px 10px":"10px 12px",boxSizing:"border-box",overflow:"hidden",boxShadow:x.shadow,isolation:"isolate"},children:u.jsx("div",{style:{display:"grid",gap:Ae,maxWidth:"100%"},children:F.map((n,i)=>{const l=Le(n.medal),c=i===0,k=i===1,R=i===2,M=oe&&(c||k||R),A=`${n.position}:${n.name}:${n.avatar||""}`,T=!!(n.avatar&&!Re.has(A)),j=u.jsxs("div",{style:{minWidth:0,display:"grid",gap:Fe,position:"relative",zIndex:2,justifyItems:s.alignRight?"end":"start"},children:[u.jsx("div",{style:{color:s.theme==="light"&&s.nameColor===g.nameColor?x.defaultNameColor:s.nameColor,fontFamily:se(s.nameFont),fontWeight:c?900:800,fontSize:c?me+2:me,lineHeight:1.1,letterSpacing:`${s.nameLetterSpacing}px`,whiteSpace:"nowrap",overflow:"hidden",textOverflow:"ellipsis",textShadow:c?"0 0 10px rgba(255,242,0,.28)":"none"},children:n.name}),u.jsxs("div",{style:{display:"flex",alignItems:"center",gap:6,justifyContent:s.alignRight?"flex-end":"flex-start",color:s.theme==="light"&&s.valueColor===g.valueColor?x.defaultValueColor:s.valueColor,fontFamily:se(s.valueFont),fontWeight:800,fontSize:$e,letterSpacing:`${s.valueLetterSpacing}px`},children:[s.showMetricIcon?Ve(o.metricIcon,c,M,s.theme==="light"&&s.valueColor===g.valueColor?x.defaultValueColor:s.valueColor,s.valueFontSize,s.valueFont):null,u.jsx("span",{className:M?"value-bump":"",style:{display:"inline-block",textShadow:c?"0 0 10px rgba(34,211,238,.22)":"none"},children:Ke(n.value)})]})]}),$=s.showAvatars?u.jsxs("div",{style:{position:"relative",width:w,height:w,minWidth:w,minHeight:w,maxWidth:w,maxHeight:w,borderRadius:"50%",overflow:"visible",border:c?"2px solid rgba(255,215,0,.95)":"2px solid rgba(255,193,7,.75)",background:x.avatarBackground,display:"grid",placeItems:"center",zIndex:2,boxShadow:c?"0 0 18px rgba(255,210,0,.45)":k||R?"0 0 12px rgba(255,255,255,.12)":"none"},children:[T?u.jsx("img",{src:n.avatar,alt:"",draggable:!1,style:{position:"absolute",inset:0,width:w,height:w,minWidth:w,minHeight:w,maxWidth:w,maxHeight:w,objectFit:"cover",objectPosition:"center",borderRadius:"50%",display:"block",lineHeight:0},referrerPolicy:"no-referrer",onError:()=>{Ie(Y=>{const O=new Set(Y);return O.add(A),O})}}):u.jsx("span",{style:{color:x.avatarInitialColor,fontSize:Math.max(18,w*.42),fontWeight:800},children:n.name.charAt(0).toUpperCase()}),s.showCrown&&n.crowned?u.jsx("span",{className:oe?"crown crown-bounce":"crown",style:{position:"absolute",top:-18,left:8,fontSize:s.compactMode?22:24,zIndex:3,filter:"drop-shadow(0 0 10px rgba(255,200,0,.55))"},children:"👑"}):null]}):null,H=s.showMedals?u.jsx("div",{style:{color:c?"#ffb72e":n.position<=3?"#ffffff":"#ff3434",fontSize:c?24:n.position<=3?20:32,fontWeight:900,lineHeight:1,textAlign:"center",position:"relative",zIndex:2,textShadow:c?"0 0 12px rgba(255,183,46,.65)":"none"},children:l||u.jsxs("span",{style:{display:"inline-block",minWidth:20},children:[n.position,"."]})}):null,E=s.alignRight?`minmax(0, 1fr)${s.showAvatars?` ${w}px`:""}${s.showMedals?` ${fe}px`:""}`:`${s.showMedals?`${fe}px `:""}${s.showAvatars?`${w}px `:""}minmax(0, 1fr)`;return u.jsxs("div",{className:["lp-rank-row",c?"top1":"",k?"top2":"",R?"top3":"",M?"event-pulse":""].join(" ").trim(),style:{display:"grid",gridTemplateColumns:E,alignItems:"center",columnGap:10,minHeight:s.compactMode?46:54,padding:c?"6px 8px":"4px 6px",borderRadius:14,background:c?x.rowBackgroundTop:x.rowBackground,border:s.theme==="light"?"1px solid rgba(15,23,42,.06)":"1px solid rgba(255,255,255,.045)",position:"relative",transform:M&&c?"scale(1.035)":"scale(1)",transition:"transform 220ms ease, filter 220ms ease"},children:[c?u.jsx("div",{className:oe?"top1-halo pulse":"top1-halo",style:{position:"absolute",inset:"2px 2px 2px 2px",borderRadius:16,pointerEvents:"none",background:x.topHalo,filter:"blur(8px)",opacity:.95}}):null,s.alignRight?u.jsxs(u.Fragment,{children:[j,$,H]}):u.jsxs(u.Fragment,{children:[H,$,j]})]},`${n.position}-${n.name}-${n.value}`)})})})}),u.jsx("style",{children:`
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
      `})]})}export{Je as RanksOverlayPage};
