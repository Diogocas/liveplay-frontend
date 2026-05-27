import{r as v,j as u}from"./index-Ca9DLf_m.js";import{n as Me,c as Te,d as Z}from"./top-record-widget-C7ZTsDow.js";import{a as he}from"./app-kv-CexorpBG.js";import{r as je}from"./overlay-url-yJYaczpp.js";import"./brazil-gifts-CAp08RMG.js";const Ee="http://127.0.0.1:35942",E={topLike:{id:"topLike",metricIcon:"heart",configKey:"liveplay.toplike.config",snapshotKey:"liveplay.toplike.snapshot",channel:"liveplay-toplike-channel",snapshotType:"top-like-sync",rankKey:"likes",queryBoard:"topLike"},topGifts:{id:"topGifts",metricIcon:"coin",configKey:"liveplay.topgifts.config",snapshotKey:"liveplay.topgifts.snapshot",channel:"liveplay-topgifts-channel",snapshotType:"top-gifts-sync",rankKey:"gifts",queryBoard:"topGifts"},weeklyRank:{id:"weeklyRank",metricIcon:"coin",configKey:"liveplay.weeklyrank.config",snapshotKey:"liveplay.weeklyrank.snapshot",channel:"liveplay-weeklyrank-channel",snapshotType:"weekly-rank-sync",rankKey:"weekly",queryBoard:"weeklyRank"},monthlyRank:{id:"monthlyRank",metricIcon:"coin",configKey:"liveplay.monthlyrank.config",snapshotKey:"liveplay.monthlyrank.snapshot",channel:"liveplay-monthlyrank-channel",snapshotType:"monthly-rank-sync",rankKey:"monthly",queryBoard:"monthlyRank"},topGift:{id:"topGift",metricIcon:"coin",configKey:"liveplay.topgift.config",snapshotKey:"liveplay.topgift.snapshot",channel:"liveplay-topgift-channel",snapshotType:"top-gift-sync",rankKey:"topGift",queryBoard:"topGift"},topCombo:{id:"topCombo",metricIcon:"coin",configKey:"liveplay.topcombo.config",snapshotKey:"liveplay.topcombo.snapshot",channel:"liveplay-topcombo-channel",snapshotType:"top-combo-sync",rankKey:"topCombo",queryBoard:"topCombo"}};function Ne(e){return e.trim().replace(/\/$/,"")}function _(e,t){return`${Ne(e)}${t.startsWith("/")?t:`/${t}`}`}const g={theme:"dark",opacity:100,topCount:10,showAvatars:!0,showMedals:!0,showCrown:!0,showMetricIcon:!0,displaySeconds:8,width:820,compactMode:!1,nameFont:"display",valueFont:"display",nameColor:"#fff200",valueColor:"#22d3ee",nameFontSize:100,valueFontSize:100,nameLetterSpacing:0,valueLetterSpacing:0,lineSpacing:100,alignRight:!1},ve={display:'"Trebuchet MS", "Arial Black", Impact, sans-serif',default:'Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',tech:'"Lucida Console", "Courier New", monospace',compact:'"Arial Narrow", "Roboto Condensed", "Segoe UI", sans-serif',serif:'Georgia, "Times New Roman", serif'};function ie(e){return ve[e]||ve.default}function ye(e){const t=typeof e=="string"?JSON.parse(e):e??{};return{...g,...t,topCount:t.topCount===3||t.topCount===5||t.topCount===10?t.topCount:g.topCount,theme:t.theme==="light"?"light":"dark",opacity:typeof t.opacity=="number"?Math.max(0,Math.min(100,t.opacity)):g.opacity,displaySeconds:typeof t.displaySeconds=="number"?Math.max(1,Math.min(60,t.displaySeconds)):g.displaySeconds,width:typeof t.width=="number"?Math.max(420,Math.min(1400,t.width)):g.width,nameFont:t.nameFont==="default"||t.nameFont==="display"||t.nameFont==="tech"||t.nameFont==="compact"||t.nameFont==="serif"?t.nameFont:g.nameFont,valueFont:t.valueFont==="default"||t.valueFont==="display"||t.valueFont==="tech"||t.valueFont==="compact"||t.valueFont==="serif"?t.valueFont:g.valueFont,nameColor:typeof t.nameColor=="string"&&t.nameColor.trim()?t.nameColor:g.nameColor,valueColor:typeof t.valueColor=="string"&&t.valueColor.trim()?t.valueColor:g.valueColor,nameFontSize:typeof t.nameFontSize=="number"?Math.max(70,Math.min(180,t.nameFontSize)):g.nameFontSize,valueFontSize:typeof t.valueFontSize=="number"?Math.max(70,Math.min(180,t.valueFontSize)):g.valueFontSize,nameLetterSpacing:typeof t.nameLetterSpacing=="number"?Math.max(-2,Math.min(12,t.nameLetterSpacing)):g.nameLetterSpacing,valueLetterSpacing:typeof t.valueLetterSpacing=="number"?Math.max(-2,Math.min(12,t.valueLetterSpacing)):g.valueLetterSpacing,lineSpacing:typeof t.lineSpacing=="number"?Math.max(70,Math.min(180,t.lineSpacing)):g.lineSpacing,alignRight:typeof t.alignRight=="boolean"?t.alignRight:g.alignRight}}function Q(e){try{const t=window.localStorage.getItem(e.configKey);return t?ye(t):g}catch{return g}}function re(e,t){const p=typeof t=="string"?JSON.parse(t):t??{};return{...Z(e.id),...p}}function X(e){try{const t=window.localStorage.getItem(e.configKey);return t?re(e,t):Z(e.id)}catch{return Z(e.id)}}function ne(e){try{const t=window.localStorage.getItem(e.snapshotKey);if(!t)return null;const p=JSON.parse(t);return p.type!==e.snapshotType?null:p}catch{return null}}function we(e){return e.id==="topLike"?"likes":e.id==="topGifts"?"gifts":e.id==="weeklyRank"?"weeklyCoins":e.id==="monthlyRank"?"monthlyCoins":e.id==="topGift"?"topGift":"topCombo"}function ze(e){return e==="gold"?"🥇":e==="silver"?"🥈":e==="bronze"?"🥉":null}function Ue(e){return new Intl.NumberFormat("pt-BR").format(e)}function Le(e){var p,o,C,y,k;const t=[e==null?void 0:e.avatar,e==null?void 0:e.avatarUrl,e==null?void 0:e.profilePictureUrl,e==null?void 0:e.profilePicture,e==null?void 0:e.photoUrl,e==null?void 0:e.pictureUrl,e==null?void 0:e.imageUrl,(p=e==null?void 0:e.user)==null?void 0:p.avatar,(o=e==null?void 0:e.user)==null?void 0:o.avatarUrl,(C=e==null?void 0:e.user)==null?void 0:C.profilePictureUrl,(y=e==null?void 0:e.user)==null?void 0:y.profilePicture,(k=e==null?void 0:e.user)==null?void 0:k.photoUrl];for(const r of t){const h=String(r||"").trim();if(h)return h}}function be(e,t,p){var y,k;const o=t==="weekly"?"weeklyCoins":t==="monthly"?"monthlyCoins":t;return(Array.isArray((k=(y=e==null?void 0:e.ranks)==null?void 0:y[o])==null?void 0:k.entries)?e.ranks[o].entries:[]).slice(0,p).map((r,h)=>{var z,f;return{position:h+1,name:String((r==null?void 0:r.nickname)||(r==null?void 0:r.username)||(r==null?void 0:r.name)||((z=r==null?void 0:r.user)==null?void 0:z.nickname)||((f=r==null?void 0:r.user)==null?void 0:f.username)||`Usuário ${h+1}`),value:Math.max(0,Number((r==null?void 0:r.value)||0)),avatar:Le(r),medal:h===0?"gold":h===1?"silver":h===2?"bronze":void 0,crowned:h===0,giftId:String((r==null?void 0:r.giftId)||"").trim()||void 0,giftName:String((r==null?void 0:r.giftName)||"").trim()||void 0,giftImageUrl:String((r==null?void 0:r.giftImageUrl)||"").trim()||void 0,comboCount:Math.max(0,Number((r==null?void 0:r.comboCount)||0))||void 0,coinValue:Math.max(0,Number((r==null?void 0:r.coinValue)||0))||void 0}})}function Ke(e,t){return e.map(p=>`${p.position}:${p.name}:${p.value}`).join("|")+`::${t||0}`}function Ge(e,t,p,o,C=100,y="display"){return e==="coin"?u.jsx("span",{className:p?"metric-icon metric-coin event-coin":"metric-icon metric-coin",style:{color:o,fontSize:(t?17:15)*(C/100),lineHeight:1,fontFamily:ie(y),filter:t?"drop-shadow(0 0 8px rgba(255,207,63,.45))":"none"},children:"●"}):u.jsx("span",{className:p?"metric-icon metric-heart event-heart":"metric-icon metric-heart",style:{color:"#ff2c55",fontSize:t?17:15,lineHeight:1,filter:t?"drop-shadow(0 0 8px rgba(255,44,85,.45))":"none"},children:"♥"})}function N(e,t){const p=Math.max(0,Math.min(1,Number(e)/100));return Math.max(0,Math.min(t,p*t))}function Pe(e,t){const p=N(t,e==="light"?.98:.88),o=N(t,e==="light"?.96:.1),C=N(t,e==="light"?.72:.035),y=N(t,.08),k=N(t,e==="light"?.16:.14),r=N(t,e==="light"?.18:.28);return e==="light"?{shellBackground:`rgba(248,250,252,${p})`,shellBorder:`1px solid rgba(15,23,42,${k})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${o}), rgba(226,232,240,${C}))`,rowBackground:`rgba(255,255,255,${C})`,avatarBackground:`rgba(15,23,42,${y})`,avatarInitialColor:"#0f172a",defaultNameColor:"#0f172a",defaultValueColor:"#0369a1",topHalo:"radial-gradient(circle at 12% 50%, rgba(59,130,246,.22), transparent 24%), radial-gradient(circle at 72% 50%, rgba(250,204,21,.22), transparent 42%)",shadow:r>0?`0 18px 36px rgba(15,23,42,${r})`:"none"}:{shellBackground:`rgba(4,7,18,${p})`,shellBorder:`1px solid rgba(255,255,255,${k})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${o}), rgba(255,255,255,${C}))`,rowBackground:`rgba(255,255,255,${C})`,avatarBackground:`rgba(255,255,255,${y})`,avatarInitialColor:"#d1d5db",defaultNameColor:"#fff200",defaultValueColor:"#22d3ee",topHalo:"radial-gradient(circle at 12% 50%, rgba(255,80,120,.35), transparent 24%), radial-gradient(circle at 72% 50%, rgba(255,195,0,.18), transparent 42%)",shadow:r>0?`0 18px 40px rgba(0,0,0,${r})`:"none"}}function qe(){var fe;const e=v.useMemo(()=>new URLSearchParams(window.location.search),[]),t=e.get("board")||"topLike",p=e.get("sourceId")||t,o=t==="topGifts"?E.topGifts:t==="weeklyRank"?E.weeklyRank:t==="monthlyRank"?E.monthlyRank:t==="topGift"?E.topGift:t==="topCombo"?E.topCombo:E.topLike,C=e.get("bridgeUrl"),y=typeof window<"u"&&typeof window.liveplay<"u",[k,r]=v.useState(()=>C||je(e,"/overlay-bridge",Ee)),[h,z]=v.useState(()=>o.id==="topGift"||o.id==="topCombo"?X(o):Q(o)),[f,O]=v.useState(()=>ne(o)),[ke,se]=v.useState([]),[Se,le]=v.useState(!1),[ee,ce]=v.useState(!1),[Ce,xe]=v.useState(()=>new Set),D=v.useRef((f==null?void 0:f.updatedAt)??0),U=v.useRef(""),te=v.useRef(Number((h==null?void 0:h.updatedAt)||0)),oe=v.useRef(!!(h!=null&&h.updatedAt)),L=a=>{if(!a){oe.current||z(o.id==="topGift"||o.id==="topCombo"?Z(o.id):g);return}const i=o.id==="topGift"||o.id==="topCombo"?re(o,a):ye(a),l=Number((i==null?void 0:i.updatedAt)||0);if(l>0){if(l<te.current)return;te.current=l,oe.current=!0}else if(oe.current)return;z(i)},I=a=>a!=null&&a.payload&&typeof a.payload=="object"?a.payload:a,ae=(a,i)=>{const l=String((a==null?void 0:a.board)||"").trim(),c=String((a==null?void 0:a.sourceId)||"").trim(),S=new Set([o.id,o.queryBoard,we(o),i||"",p].filter(Boolean));return!(l&&!S.has(l)||c&&!S.has(c))},K=a=>{var c,S;const i=I(a);if(!i||i.type!=="rank-config-sync"||!ae(i))return;const l=Number(i.updatedAt||((c=i.config)==null?void 0:c.updatedAt)||0);l&&l<te.current||L({...i.config,updatedAt:l||Number(((S=i.config)==null?void 0:S.updatedAt)||Date.now())})};v.useEffect(()=>{const a=document.documentElement,i=document.body,l=document.getElementById("root"),c=a.style.background,S=i.style.background,R=(l==null?void 0:l.style.background)??"";return a.style.background="transparent",i.style.background="transparent",l&&(l.style.background="transparent"),()=>{a.style.background=c,i.style.background=S,l&&(l.style.background=R)}},[]),v.useEffect(()=>{var a,i,l;y&&((l=(i=(a=window.liveplay)==null?void 0:a.app)==null?void 0:i.getOverlayBridgeUrl)==null||l.call(i).then(c=>{c!=null&&c.url&&r(c.url)}).catch(()=>{}))},[y]),v.useEffect(()=>{L(o.id==="topGift"||o.id==="topCombo"?X(o):Q(o));const a=ne(o);O(a),D.current=(a==null?void 0:a.updatedAt)??0},[o]),v.useEffect(()=>{let a=!0;return(async()=>{const[l,c]=await Promise.all([he(o.configKey,null),he(o.snapshotKey,null)]);a&&(l&&L(l),c&&c.type===o.snapshotType&&(!c.board||c.board===o.id)&&(O(c),D.current=c.updatedAt??0))})().catch(()=>{}),()=>{a=!1}},[o]),v.useEffect(()=>{var me,q,ge;const a=()=>L(o.id==="topGift"||o.id==="topCombo"?X(o):Q(o)),i=d=>{const n=I(d);if(!n||n.type!==o.snapshotType||!ae(n))return;d=n;const b=Number(d.updatedAt||d.at||Date.now());b<D.current||(D.current=b,O({...d,updatedAt:b}))},l=d=>{d.key===o.configKey&&a(),d.key===o.snapshotKey&&i(ne(o))},c=d=>{i(d.detail)},S=d=>{K(d.detail)};window.addEventListener("storage",l),window.addEventListener(`liveplay:${o.id}-snapshot`,c),window.addEventListener(`liveplay:${o.id}-config`,S);let R=null;try{R=new BroadcastChannel(o.channel),R.onmessage=d=>{const n=I(d.data);if((n==null?void 0:n.type)==="rank-config-sync"){K(n);return}i(n)}}catch{}const F=window.setInterval(()=>{document.visibilityState==="visible"&&L(o.id==="topGift"||o.id==="topCombo"?X(o):Q(o))},2500);let B=!1;const P=we(o),M=d=>{const n=I(d);if(!n||(n==null?void 0:n.type)!=="rank-overlay-sync"||!ae(n,P))return;const b=n==null?void 0:n.ranks;b&&(le(!!(n!=null&&n.liveActive)),se(be({ranks:b},o.rankKey,10)))},A=async()=>{try{const[d,n,b]=await Promise.all([fetch(_(k,`/snapshot?type=rank-config-sync&board=${o.id}&sourceId=${encodeURIComponent(p)}`),{cache:"no-store"}).then(H=>H.json()).catch(()=>null),fetch(_(k,`/snapshot?type=${o.snapshotType}&board=${o.id}&sourceId=${encodeURIComponent(p)}`),{cache:"no-store"}).then(H=>H.json()).catch(()=>null),fetch(_(k,`/snapshot?type=rank-overlay-sync&board=${P}&sourceId=${encodeURIComponent(p)}`),{cache:"no-store"}).then(H=>H.json()).catch(()=>null)]);if(B)return;const m=I((d==null?void 0:d.payload)??d);(m==null?void 0:m.type)==="rank-config-sync"&&K(m);const j=I((n==null?void 0:n.payload)??n);(j==null?void 0:j.type)===o.snapshotType&&i({...j,updatedAt:Number(j.updatedAt||j.at||Date.now())});const J=I((b==null?void 0:b.payload)??b);(J==null?void 0:J.type)==="rank-overlay-sync"&&M(J)}catch{}},V=[],T=d=>{try{const n=new EventSource(_(k,d));n.onmessage=b=>{try{const m=I(JSON.parse(b.data));if((m==null?void 0:m.type)==="rank-config-sync"){K(m);return}if((m==null?void 0:m.type)===o.snapshotType){i({...m,updatedAt:Number(m.updatedAt||m.at||Date.now())});return}(m==null?void 0:m.type)==="rank-overlay-sync"&&M(m)}catch{}},V.push(n)}catch{}};T(`/events?type=rank-config-sync&board=${encodeURIComponent(o.id)}&sourceId=${encodeURIComponent(p)}`),T(`/events?type=${encodeURIComponent(o.snapshotType)}&board=${encodeURIComponent(o.id)}&sourceId=${encodeURIComponent(p)}`),T(`/events?type=rank-overlay-sync&board=${encodeURIComponent(P)}&sourceId=${encodeURIComponent(p)}`),T("/events");const Y=(ge=(q=(me=window.liveplay)==null?void 0:me.app)==null?void 0:q.onOverlaySync)==null?void 0:ge.call(q,d=>{const n=I(d);if((n==null?void 0:n.type)==="rank-config-sync"){K(n);return}if((n==null?void 0:n.type)===o.snapshotType){i({...n,updatedAt:Number(n.updatedAt||n.at||Date.now())});return}(n==null?void 0:n.type)==="rank-overlay-sync"&&M(n)}),W=async()=>{var d;try{const n=(d=window.liveplay)==null?void 0:d.app;if(!(n!=null&&n.getState)||!(n!=null&&n.getStatus)){await A();return}const[b,m]=await Promise.all([n.getState(),n.getStatus()]);if(B)return;le(!!(m!=null&&m.tiktokConnected)),se(be(b,o.rankKey,10)),await A()}catch{B||await A()}};W();const $e=[80,220,500,900,1500,2400].map(d=>window.setTimeout(()=>{B||A()},d)),Fe=window.setInterval(()=>{document.visibilityState==="visible"&&W()},5e3);return()=>{B=!0,$e.forEach(d=>window.clearTimeout(d)),window.removeEventListener("storage",l),window.removeEventListener(`liveplay:${o.id}-snapshot`,c),window.removeEventListener(`liveplay:${o.id}-config`,S),window.clearInterval(F),window.clearInterval(Fe),R==null||R.close(),V.forEach(d=>d.close()),typeof Y=="function"&&Y()}},[k,y,o,p]);const de=o.id==="topGift"||o.id==="topCombo"?1:h.topCount,Re=o.id==="weeklyRank"||o.id==="monthlyRank",G=!!(f!=null&&f.visible)&&f.mode==="test"&&((fe=f.entries)==null?void 0:fe.length),$=G?f.entries.slice(0,de):Se||Re?ke.slice(0,de):[];if(v.useEffect(()=>{if(!G||!f)return;const a=o.id==="topGift"||o.id==="topCombo"?8:Math.max(1,Math.min(60,Number(h.displaySeconds||g.displaySeconds))),i=window.setTimeout(()=>{O(l=>!l||l.mode!=="test"||l.updatedAt!==f.updatedAt?l:{...l,visible:!1,updatedAt:Date.now()})},a*1e3+450);return()=>window.clearTimeout(i)},[G,f==null?void 0:f.updatedAt,o.id,h]),v.useEffect(()=>{const a=Ke($,G?f==null?void 0:f.updatedAt:0);if(!a){U.current="";return}if(U.current&&U.current!==a){ce(!0);const i=window.setTimeout(()=>ce(!1),950);return U.current=a,()=>window.clearTimeout(i)}U.current=a},[$,G,f==null?void 0:f.updatedAt]),(o.id==="topGift"||o.id==="topCombo")&&$.length){const a=re(o,h),i=$[0],l=Me({username:i.name,nickname:i.name,avatarUrl:i.avatar,giftId:i.giftId,giftName:i.giftName,giftImageUrl:i.giftImageUrl,comboCount:i.comboCount,coinValue:i.coinValue,value:i.value},o.id);return l?u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:"flex-start",padding:0,overflow:"hidden"},children:u.jsx("div",{style:{width:390,maxWidth:"100%",padding:10,boxSizing:"border-box",background:"transparent",border:"none",borderRadius:16,boxShadow:"none"},children:u.jsx(Te,{kind:o.id,config:a,entry:l,embedded:!0})})}):u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}})}if(!$.length)return u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}});const s=h,Ie=s.compactMode?8:10,w=s.compactMode?42:48,pe=s.compactMode?30:34,ue=(s.compactMode?13:15)*(s.nameFontSize/100),Be=(s.compactMode?12:14)*(s.valueFontSize/100),Ae=2*(s.lineSpacing/100),x=Pe(s.theme,s.opacity);return u.jsxs(u.Fragment,{children:[u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:s.alignRight?"flex-end":"flex-start",padding:0,overflow:"hidden"},children:u.jsx("div",{style:{width:Math.min(s.width,1400),minHeight:180,borderRadius:16,border:x.shellBorder,background:x.shellBackground,padding:s.compactMode?"8px 10px":"10px 12px",boxSizing:"border-box",overflow:"hidden",boxShadow:x.shadow,isolation:"isolate"},children:u.jsx("div",{style:{display:"grid",gap:Ie,maxWidth:"100%"},children:$.map((a,i)=>{const l=ze(a.medal),c=i===0,S=i===1,R=i===2,F=ee&&(c||S||R),B=`${a.position}:${a.name}:${a.avatar||""}`,P=!!(a.avatar&&!Ce.has(B)),M=u.jsxs("div",{style:{minWidth:0,display:"grid",gap:Ae,position:"relative",zIndex:2,justifyItems:s.alignRight?"end":"start"},children:[u.jsx("div",{style:{color:s.theme==="light"&&s.nameColor===g.nameColor?x.defaultNameColor:s.nameColor,fontFamily:ie(s.nameFont),fontWeight:c?900:800,fontSize:c?ue+2:ue,lineHeight:1.1,letterSpacing:`${s.nameLetterSpacing}px`,whiteSpace:"nowrap",overflow:"hidden",textOverflow:"ellipsis",textShadow:c?"0 0 10px rgba(255,242,0,.28)":"none"},children:a.name}),u.jsxs("div",{style:{display:"flex",alignItems:"center",gap:6,justifyContent:s.alignRight?"flex-end":"flex-start",color:s.theme==="light"&&s.valueColor===g.valueColor?x.defaultValueColor:s.valueColor,fontFamily:ie(s.valueFont),fontWeight:800,fontSize:Be,letterSpacing:`${s.valueLetterSpacing}px`},children:[s.showMetricIcon?Ge(o.metricIcon,c,F,s.theme==="light"&&s.valueColor===g.valueColor?x.defaultValueColor:s.valueColor,s.valueFontSize,s.valueFont):null,u.jsx("span",{className:F?"value-bump":"",style:{display:"inline-block",textShadow:c?"0 0 10px rgba(34,211,238,.22)":"none"},children:Ue(a.value)})]})]}),A=s.showAvatars?u.jsxs("div",{style:{position:"relative",width:w,height:w,minWidth:w,minHeight:w,maxWidth:w,maxHeight:w,borderRadius:"50%",overflow:"visible",border:c?"2px solid rgba(255,215,0,.95)":"2px solid rgba(255,193,7,.75)",background:x.avatarBackground,display:"grid",placeItems:"center",zIndex:2,boxShadow:c?"0 0 18px rgba(255,210,0,.45)":S||R?"0 0 12px rgba(255,255,255,.12)":"none"},children:[P?u.jsx("img",{src:a.avatar,alt:"",draggable:!1,style:{position:"absolute",inset:0,width:w,height:w,minWidth:w,minHeight:w,maxWidth:w,maxHeight:w,objectFit:"cover",objectPosition:"center",borderRadius:"50%",display:"block",lineHeight:0},referrerPolicy:"no-referrer",onError:()=>{xe(Y=>{const W=new Set(Y);return W.add(B),W})}}):u.jsx("span",{style:{color:x.avatarInitialColor,fontSize:Math.max(18,w*.42),fontWeight:800},children:a.name.charAt(0).toUpperCase()}),s.showCrown&&a.crowned?u.jsx("span",{className:ee?"crown crown-bounce":"crown",style:{position:"absolute",top:-18,left:8,fontSize:s.compactMode?22:24,zIndex:3,filter:"drop-shadow(0 0 10px rgba(255,200,0,.55))"},children:"👑"}):null]}):null,V=s.showMedals?u.jsx("div",{style:{color:c?"#ffb72e":a.position<=3?"#ffffff":"#ff3434",fontSize:c?24:a.position<=3?20:32,fontWeight:900,lineHeight:1,textAlign:"center",position:"relative",zIndex:2,textShadow:c?"0 0 12px rgba(255,183,46,.65)":"none"},children:l||u.jsxs("span",{style:{display:"inline-block",minWidth:20},children:[a.position,"."]})}):null,T=s.alignRight?`minmax(0, 1fr)${s.showAvatars?` ${w}px`:""}${s.showMedals?` ${pe}px`:""}`:`${s.showMedals?`${pe}px `:""}${s.showAvatars?`${w}px `:""}minmax(0, 1fr)`;return u.jsxs("div",{className:["lp-rank-row",c?"top1":"",S?"top2":"",R?"top3":"",F?"event-pulse":""].join(" ").trim(),style:{display:"grid",gridTemplateColumns:T,alignItems:"center",columnGap:10,minHeight:s.compactMode?46:54,padding:c?"6px 8px":"4px 6px",borderRadius:14,background:c?x.rowBackgroundTop:x.rowBackground,border:s.theme==="light"?"1px solid rgba(15,23,42,.06)":"1px solid rgba(255,255,255,.045)",position:"relative",transform:F&&c?"scale(1.035)":"scale(1)",transition:"transform 220ms ease, filter 220ms ease"},children:[c?u.jsx("div",{className:ee?"top1-halo pulse":"top1-halo",style:{position:"absolute",inset:"2px 2px 2px 2px",borderRadius:16,pointerEvents:"none",background:x.topHalo,filter:"blur(8px)",opacity:.95}}):null,s.alignRight?u.jsxs(u.Fragment,{children:[M,A,V]}):u.jsxs(u.Fragment,{children:[V,A,M]})]},`${a.position}-${a.name}-${a.value}`)})})})}),u.jsx("style",{children:`
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
      `})]})}export{qe as RanksOverlayPage};
