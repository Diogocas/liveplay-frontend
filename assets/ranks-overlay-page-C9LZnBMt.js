import{r as h,j as u}from"./index-CAhuBTns.js";import{n as Ce,c as Re,d as q}from"./top-record-widget-Clsh67FM.js";import{g as fe}from"./app-kv-BJKjXjFm.js";import{r as Be}from"./overlay-url-IdlNOR_r.js";import"./brazil-gifts-DLMOKDcl.js";const Ae="http://127.0.0.1:35942",M={topLike:{id:"topLike",metricIcon:"heart",configKey:"liveplay.toplike.config",snapshotKey:"liveplay.toplike.snapshot",channel:"liveplay-toplike-channel",snapshotType:"top-like-sync",rankKey:"likes",queryBoard:"topLike"},topGifts:{id:"topGifts",metricIcon:"coin",configKey:"liveplay.topgifts.config",snapshotKey:"liveplay.topgifts.snapshot",channel:"liveplay-topgifts-channel",snapshotType:"top-gifts-sync",rankKey:"gifts",queryBoard:"topGifts"},weeklyRank:{id:"weeklyRank",metricIcon:"coin",configKey:"liveplay.weeklyrank.config",snapshotKey:"liveplay.weeklyrank.snapshot",channel:"liveplay-weeklyrank-channel",snapshotType:"weekly-rank-sync",rankKey:"weekly",queryBoard:"weeklyRank"},monthlyRank:{id:"monthlyRank",metricIcon:"coin",configKey:"liveplay.monthlyrank.config",snapshotKey:"liveplay.monthlyrank.snapshot",channel:"liveplay-monthlyrank-channel",snapshotType:"monthly-rank-sync",rankKey:"monthly",queryBoard:"monthlyRank"},topGift:{id:"topGift",metricIcon:"coin",configKey:"liveplay.topgift.config",snapshotKey:"liveplay.topgift.snapshot",channel:"liveplay-topgift-channel",snapshotType:"top-gift-sync",rankKey:"topGift",queryBoard:"topGift"},topCombo:{id:"topCombo",metricIcon:"coin",configKey:"liveplay.topcombo.config",snapshotKey:"liveplay.topcombo.snapshot",channel:"liveplay-topcombo-channel",snapshotType:"top-combo-sync",rankKey:"topCombo",queryBoard:"topCombo"}};function Ie(o){return o.trim().replace(/\/$/,"")}function D(o,e){return`${Ie(o)}${e.startsWith("/")?e:`/${e}`}`}const f={theme:"dark",opacity:100,topCount:10,showAvatars:!0,showMedals:!0,showCrown:!0,showMetricIcon:!0,displaySeconds:8,width:820,compactMode:!1,nameFont:"display",valueFont:"display",nameColor:"#fff200",valueColor:"#22d3ee",nameFontSize:100,valueFontSize:100,nameLetterSpacing:0,valueLetterSpacing:0,lineSpacing:100,alignRight:!1},me={display:'"Trebuchet MS", "Arial Black", Impact, sans-serif',default:'Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',tech:'"Lucida Console", "Courier New", monospace',compact:'"Arial Narrow", "Roboto Condensed", "Segoe UI", sans-serif',serif:'Georgia, "Times New Roman", serif'};function Z(o){return me[o]||me.default}function he(o){const e=typeof o=="string"?JSON.parse(o):o??{};return{...f,...e,topCount:e.topCount===3||e.topCount===5||e.topCount===10?e.topCount:f.topCount,theme:e.theme==="light"?"light":"dark",opacity:typeof e.opacity=="number"?Math.max(0,Math.min(100,e.opacity)):f.opacity,displaySeconds:typeof e.displaySeconds=="number"?Math.max(1,Math.min(60,e.displaySeconds)):f.displaySeconds,width:typeof e.width=="number"?Math.max(420,Math.min(1400,e.width)):f.width,nameFont:e.nameFont==="default"||e.nameFont==="display"||e.nameFont==="tech"||e.nameFont==="compact"||e.nameFont==="serif"?e.nameFont:f.nameFont,valueFont:e.valueFont==="default"||e.valueFont==="display"||e.valueFont==="tech"||e.valueFont==="compact"||e.valueFont==="serif"?e.valueFont:f.valueFont,nameColor:typeof e.nameColor=="string"&&e.nameColor.trim()?e.nameColor:f.nameColor,valueColor:typeof e.valueColor=="string"&&e.valueColor.trim()?e.valueColor:f.valueColor,nameFontSize:typeof e.nameFontSize=="number"?Math.max(70,Math.min(180,e.nameFontSize)):f.nameFontSize,valueFontSize:typeof e.valueFontSize=="number"?Math.max(70,Math.min(180,e.valueFontSize)):f.valueFontSize,nameLetterSpacing:typeof e.nameLetterSpacing=="number"?Math.max(-2,Math.min(12,e.nameLetterSpacing)):f.nameLetterSpacing,valueLetterSpacing:typeof e.valueLetterSpacing=="number"?Math.max(-2,Math.min(12,e.valueLetterSpacing)):f.valueLetterSpacing,lineSpacing:typeof e.lineSpacing=="number"?Math.max(70,Math.min(180,e.lineSpacing)):f.lineSpacing,alignRight:typeof e.alignRight=="boolean"?e.alignRight:f.alignRight}}function O(o){try{const e=window.localStorage.getItem(o.configKey);return e?he(e):f}catch{return f}}function ee(o,e){const p=typeof e=="string"?JSON.parse(e):e??{};return{...q(o.id),...p}}function Y(o){try{const e=window.localStorage.getItem(o.configKey);return e?ee(o,e):q(o.id)}catch{return q(o.id)}}function X(o){try{const e=window.localStorage.getItem(o.snapshotKey);if(!e)return null;const p=JSON.parse(e);return p.type!==o.snapshotType?null:p}catch{return null}}function Fe(o){return o.id==="topLike"?"likes":o.id==="topGifts"?"gifts":o.id==="weeklyRank"?"weeklyCoins":o.id==="monthlyRank"?"monthlyCoins":o.id==="topGift"?"topGift":"topCombo"}function Me(o){return o==="gold"?"🥇":o==="silver"?"🥈":o==="bronze"?"🥉":null}function $e(o){return new Intl.NumberFormat("pt-BR").format(o)}function ge(o,e,p){var v,b;const t=e==="weekly"?"weeklyCoins":e==="monthly"?"monthlyCoins":e;return(Array.isArray((b=(v=o==null?void 0:o.ranks)==null?void 0:v[t])==null?void 0:b.entries)?o.ranks[t].entries:[]).slice(0,p).map((d,g)=>({position:g+1,name:String((d==null?void 0:d.nickname)||(d==null?void 0:d.username)||`Usuário ${g+1}`),value:Math.max(0,Number((d==null?void 0:d.value)||0)),avatar:String((d==null?void 0:d.avatarUrl)||"").trim()||void 0,medal:g===0?"gold":g===1?"silver":g===2?"bronze":void 0,crowned:g===0,giftId:String((d==null?void 0:d.giftId)||"").trim()||void 0,giftName:String((d==null?void 0:d.giftName)||"").trim()||void 0,giftImageUrl:String((d==null?void 0:d.giftImageUrl)||"").trim()||void 0,comboCount:Math.max(0,Number((d==null?void 0:d.comboCount)||0))||void 0,coinValue:Math.max(0,Number((d==null?void 0:d.coinValue)||0))||void 0}))}function je(o,e){return o.map(p=>`${p.position}:${p.name}:${p.value}`).join("|")+`::${e||0}`}function Te(o,e,p,t,k=100,v="display"){return o==="coin"?u.jsx("span",{className:p?"metric-icon metric-coin event-coin":"metric-icon metric-coin",style:{color:t,fontSize:(e?17:15)*(k/100),lineHeight:1,fontFamily:Z(v),filter:e?"drop-shadow(0 0 8px rgba(255,207,63,.45))":"none"},children:"●"}):u.jsx("span",{className:p?"metric-icon metric-heart event-heart":"metric-icon metric-heart",style:{color:"#ff2c55",fontSize:e?17:15,lineHeight:1,filter:e?"drop-shadow(0 0 8px rgba(255,44,85,.45))":"none"},children:"♥"})}function $(o,e){const p=Math.max(0,Math.min(1,Number(o)/100));return Math.max(0,Math.min(e,p*e))}function Ne(o,e){const p=$(e,o==="light"?.98:.88),t=$(e,o==="light"?.96:.1),k=$(e,o==="light"?.72:.035),v=$(e,.08),b=$(e,o==="light"?.16:.14),d=$(e,o==="light"?.18:.28);return o==="light"?{shellBackground:`rgba(248,250,252,${p})`,shellBorder:`1px solid rgba(15,23,42,${b})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${t}), rgba(226,232,240,${k}))`,rowBackground:`rgba(255,255,255,${k})`,avatarBackground:`rgba(15,23,42,${v})`,avatarInitialColor:"#0f172a",defaultNameColor:"#0f172a",defaultValueColor:"#0369a1",topHalo:"radial-gradient(circle at 12% 50%, rgba(59,130,246,.22), transparent 24%), radial-gradient(circle at 72% 50%, rgba(250,204,21,.22), transparent 42%)",shadow:d>0?`0 18px 36px rgba(15,23,42,${d})`:"none"}:{shellBackground:`rgba(4,7,18,${p})`,shellBorder:`1px solid rgba(255,255,255,${b})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${t}), rgba(255,255,255,${k}))`,rowBackground:`rgba(255,255,255,${k})`,avatarBackground:`rgba(255,255,255,${v})`,avatarInitialColor:"#d1d5db",defaultNameColor:"#fff200",defaultValueColor:"#22d3ee",topHalo:"radial-gradient(circle at 12% 50%, rgba(255,80,120,.35), transparent 24%), radial-gradient(circle at 72% 50%, rgba(255,195,0,.18), transparent 42%)",shadow:d>0?`0 18px 40px rgba(0,0,0,${d})`:"none"}}function Pe(){var le;const o=h.useMemo(()=>new URLSearchParams(window.location.search),[]),e=o.get("board")||"topLike",p=o.get("sourceId")||e,t=e==="topGifts"?M.topGifts:e==="weeklyRank"?M.weeklyRank:e==="monthlyRank"?M.monthlyRank:e==="topGift"?M.topGift:e==="topCombo"?M.topCombo:M.topLike,k=o.get("bridgeUrl"),v=typeof window<"u"&&typeof window.liveplay<"u",[b,d]=h.useState(()=>k||Be(o,"/overlay-bridge",Ae)),[g,te]=h.useState(()=>t.id==="topGift"||t.id==="topCombo"?Y(t):O(t)),[m,P]=h.useState(()=>X(t)),[ve,ne]=h.useState([]),[be,oe]=h.useState(!1),[J,ae]=h.useState(!1),V=h.useRef((m==null?void 0:m.updatedAt)??0),j=h.useRef(""),_=h.useRef(Number((g==null?void 0:g.updatedAt)||0)),Q=h.useRef(!!(g!=null&&g.updatedAt)),T=a=>{if(!a){Q.current||te(t.id==="topGift"||t.id==="topCombo"?q(t.id):f);return}const s=t.id==="topGift"||t.id==="topCombo"?ee(t,a):he(a),l=Number((s==null?void 0:s.updatedAt)||0);if(l>0){if(l<_.current)return;_.current=l,Q.current=!0}else if(Q.current)return;te(s)},N=a=>{var l,c;if(!a||a.type!=="rank-config-sync"||a.board!==t.id)return;const s=Number(a.updatedAt||((l=a.config)==null?void 0:l.updatedAt)||0);s&&s<_.current||T({...a.config,updatedAt:s||Number(((c=a.config)==null?void 0:c.updatedAt)||Date.now())})};h.useEffect(()=>{const a=document.documentElement,s=document.body,l=document.getElementById("root"),c=a.style.background,x=s.style.background,y=(l==null?void 0:l.style.background)??"";return a.style.background="transparent",s.style.background="transparent",l&&(l.style.background="transparent"),()=>{a.style.background=c,s.style.background=x,l&&(l.style.background=y)}},[]),h.useEffect(()=>{var a,s,l;v&&((l=(s=(a=window.liveplay)==null?void 0:a.app)==null?void 0:s.getOverlayBridgeUrl)==null||l.call(s).then(c=>{c!=null&&c.url&&d(c.url)}).catch(()=>{}))},[v]),h.useEffect(()=>{T(t.id==="topGift"||t.id==="topCombo"?Y(t):O(t));const a=X(t);P(a),V.current=(a==null?void 0:a.updatedAt)??0},[t]),h.useEffect(()=>{let a=!0;return(async()=>{const[l,c]=await Promise.all([fe(t.configKey,null),fe(t.snapshotKey,null)]);a&&(l&&T(l),c&&c.type===t.snapshotType&&(!c.board||c.board===t.id)&&(P(c),V.current=c.updatedAt??0))})().catch(()=>{}),()=>{a=!1}},[t]),h.useEffect(()=>{var ue,W,pe;const a=()=>T(t.id==="topGift"||t.id==="topCombo"?Y(t):O(t)),s=n=>{if(!n||n.type!==t.snapshotType||typeof n.board=="string"&&n.board&&n.board!==t.id)return;const r=Number(n.updatedAt||n.at||Date.now());r<V.current||(V.current=r,P({...n,updatedAt:r}))},l=n=>{n.key===t.configKey&&a(),n.key===t.snapshotKey&&s(X(t))},c=n=>{s(n.detail)},x=n=>{N(n.detail)};window.addEventListener("storage",l),window.addEventListener(`liveplay:${t.id}-snapshot`,c),window.addEventListener(`liveplay:${t.id}-config`,x);let y=null;try{y=new BroadcastChannel(t.channel),y.onmessage=n=>{const r=n.data;if((r==null?void 0:r.type)==="rank-config-sync"){N(r);return}s(r)}}catch{}const B=window.setInterval(()=>{document.visibilityState==="visible"&&T(t.id==="topGift"||t.id==="topCombo"?Y(t):O(t))},2500);let C=!1;const E=Fe(t),A=n=>{if(!n||(n==null?void 0:n.type)!=="rank-overlay-sync"||typeof(n==null?void 0:n.board)=="string"&&n.board&&n.board!==E)return;const r=n==null?void 0:n.ranks;r&&(oe(!!(n!=null&&n.liveActive)),ne(ge({ranks:r},t.rankKey,10)))},G=async()=>{try{const[n,r,K]=await Promise.all([fetch(D(b,`/snapshot?type=rank-config-sync&board=${t.id}&sourceId=${encodeURIComponent(p)}`),{cache:"no-store"}).then(U=>U.json()).catch(()=>null),fetch(D(b,`/snapshot?type=${t.snapshotType}&board=${t.id}&sourceId=${encodeURIComponent(p)}`),{cache:"no-store"}).then(U=>U.json()).catch(()=>null),fetch(D(b,`/snapshot?type=rank-overlay-sync&board=${E}&sourceId=${encodeURIComponent(p)}`),{cache:"no-store"}).then(U=>U.json()).catch(()=>null)]);if(C)return;const S=n==null?void 0:n.payload;(S==null?void 0:S.type)==="rank-config-sync"&&N(S);const F=r==null?void 0:r.payload;(F==null?void 0:F.type)===t.snapshotType&&s({...F,updatedAt:Number(F.updatedAt||F.at||Date.now())});const H=K==null?void 0:K.payload;(H==null?void 0:H.type)==="rank-overlay-sync"&&A(H)}catch{}};let I=null;try{I=new EventSource(D(b,"/events")),I.onmessage=n=>{try{const r=JSON.parse(n.data);if((r==null?void 0:r.type)==="rank-config-sync"){N(r);return}if((r==null?void 0:r.type)===t.snapshotType){if(typeof(r==null?void 0:r.board)=="string"&&r.board&&r.board!==t.id)return;s({...r,updatedAt:Number(r.updatedAt||r.at||Date.now())});return}(r==null?void 0:r.type)==="rank-overlay-sync"&&A(r)}catch{}}}catch{I=null}const ce=(pe=(W=(ue=window.liveplay)==null?void 0:ue.app)==null?void 0:W.onOverlaySync)==null?void 0:pe.call(W,n=>{if((n==null?void 0:n.type)==="rank-config-sync"){N(n);return}if((n==null?void 0:n.type)===t.snapshotType){if(typeof(n==null?void 0:n.board)=="string"&&n.board&&n.board!==t.id)return;s({...n,updatedAt:Number(n.updatedAt||n.at||Date.now())});return}(n==null?void 0:n.type)==="rank-overlay-sync"&&A(n)}),de=async()=>{var n;try{const r=(n=window.liveplay)==null?void 0:n.app;if(!(r!=null&&r.getState)||!(r!=null&&r.getStatus)){await G();return}const[K,S]=await Promise.all([r.getState(),r.getStatus()]);if(C)return;oe(!!(S!=null&&S.tiktokConnected)),ne(ge(K,t.rankKey,10)),await G()}catch{C||await G()}};de();const Se=window.setInterval(()=>{document.visibilityState==="visible"&&de()},900);return()=>{C=!0,window.removeEventListener("storage",l),window.removeEventListener(`liveplay:${t.id}-snapshot`,c),window.removeEventListener(`liveplay:${t.id}-config`,x),window.clearInterval(B),window.clearInterval(Se),y==null||y.close(),I==null||I.close(),typeof ce=="function"&&ce()}},[b,v,t,p]);const re=t.id==="topGift"||t.id==="topCombo"?1:g.topCount,we=t.id==="weeklyRank"||t.id==="monthlyRank",z=!!(m!=null&&m.visible)&&m.mode==="test"&&((le=m.entries)==null?void 0:le.length),R=z?m.entries.slice(0,re):be||we?ve.slice(0,re):[];if(h.useEffect(()=>{if(!z||!m)return;const a=t.id==="topGift"||t.id==="topCombo"?8:Math.max(1,Math.min(60,Number(g.displaySeconds||f.displaySeconds))),s=window.setTimeout(()=>{P(l=>!l||l.mode!=="test"||l.updatedAt!==m.updatedAt?l:{...l,visible:!1,updatedAt:Date.now()})},a*1e3+450);return()=>window.clearTimeout(s)},[z,m==null?void 0:m.updatedAt,t.id,g]),h.useEffect(()=>{const a=je(R,z?m==null?void 0:m.updatedAt:0);if(!a){j.current="";return}if(j.current&&j.current!==a){ae(!0);const s=window.setTimeout(()=>ae(!1),950);return j.current=a,()=>window.clearTimeout(s)}j.current=a},[R,z,m==null?void 0:m.updatedAt]),(t.id==="topGift"||t.id==="topCombo")&&R.length){const a=ee(t,g),s=R[0],l=Ce({username:s.name,nickname:s.name,avatarUrl:s.avatar,giftId:s.giftId,giftName:s.giftName,giftImageUrl:s.giftImageUrl,comboCount:s.comboCount,coinValue:s.coinValue,value:s.value},t.id);return l?u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:"flex-start",padding:0,overflow:"hidden"},children:u.jsx("div",{style:{width:390,maxWidth:"100%",padding:10,boxSizing:"border-box",background:"transparent",border:"none",borderRadius:16,boxShadow:"none"},children:u.jsx(Re,{kind:t.id,config:a,entry:l,embedded:!0})})}):u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}})}if(!R.length)return u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}});const i=g,ye=i.compactMode?8:10,L=i.compactMode?42:48,ie=i.compactMode?30:34,se=(i.compactMode?13:15)*(i.nameFontSize/100),ke=(i.compactMode?12:14)*(i.valueFontSize/100),xe=2*(i.lineSpacing/100),w=Ne(i.theme,i.opacity);return u.jsxs(u.Fragment,{children:[u.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:i.alignRight?"flex-end":"flex-start",padding:0,overflow:"hidden"},children:u.jsx("div",{style:{width:Math.min(i.width,1400),minHeight:180,borderRadius:16,border:w.shellBorder,background:w.shellBackground,padding:i.compactMode?"8px 10px":"10px 12px",boxSizing:"border-box",overflow:"hidden",boxShadow:w.shadow,isolation:"isolate"},children:u.jsx("div",{style:{display:"grid",gap:ye,maxWidth:"100%"},children:R.map((a,s)=>{const l=Me(a.medal),c=s===0,x=s===1,y=s===2,B=J&&(c||x||y),C=u.jsxs("div",{style:{minWidth:0,display:"grid",gap:xe,position:"relative",zIndex:2,justifyItems:i.alignRight?"end":"start"},children:[u.jsx("div",{style:{color:i.theme==="light"&&i.nameColor===f.nameColor?w.defaultNameColor:i.nameColor,fontFamily:Z(i.nameFont),fontWeight:c?900:800,fontSize:c?se+2:se,lineHeight:1.1,letterSpacing:`${i.nameLetterSpacing}px`,whiteSpace:"nowrap",overflow:"hidden",textOverflow:"ellipsis",textShadow:c?"0 0 10px rgba(255,242,0,.28)":"none"},children:a.name}),u.jsxs("div",{style:{display:"flex",alignItems:"center",gap:6,justifyContent:i.alignRight?"flex-end":"flex-start",color:i.theme==="light"&&i.valueColor===f.valueColor?w.defaultValueColor:i.valueColor,fontFamily:Z(i.valueFont),fontWeight:800,fontSize:ke,letterSpacing:`${i.valueLetterSpacing}px`},children:[i.showMetricIcon?Te(t.metricIcon,c,B,i.theme==="light"&&i.valueColor===f.valueColor?w.defaultValueColor:i.valueColor,i.valueFontSize,i.valueFont):null,u.jsx("span",{className:B?"value-bump":"",style:{display:"inline-block",textShadow:c?"0 0 10px rgba(34,211,238,.22)":"none"},children:$e(a.value)})]})]}),E=i.showAvatars?u.jsxs("div",{style:{position:"relative",width:L,height:L,borderRadius:"50%",overflow:"visible",border:c?"2px solid rgba(255,215,0,.95)":"2px solid rgba(255,193,7,.75)",background:w.avatarBackground,display:"grid",placeItems:"center",zIndex:2,boxShadow:c?"0 0 18px rgba(255,210,0,.45)":x||y?"0 0 12px rgba(255,255,255,.12)":"none"},children:[a.avatar?u.jsx("img",{src:a.avatar,alt:a.name,style:{width:"100%",height:"100%",objectFit:"cover",borderRadius:"50%"},referrerPolicy:"no-referrer"}):u.jsx("span",{style:{color:w.avatarInitialColor,fontSize:Math.max(18,L*.42),fontWeight:800},children:a.name.charAt(0).toUpperCase()}),i.showCrown&&a.crowned?u.jsx("span",{className:J?"crown crown-bounce":"crown",style:{position:"absolute",top:-18,left:8,fontSize:i.compactMode?22:24,zIndex:3,filter:"drop-shadow(0 0 10px rgba(255,200,0,.55))"},children:"👑"}):null]}):null,A=i.showMedals?u.jsx("div",{style:{color:c?"#ffb72e":a.position<=3?"#ffffff":"#ff3434",fontSize:c?24:a.position<=3?20:32,fontWeight:900,lineHeight:1,textAlign:"center",position:"relative",zIndex:2,textShadow:c?"0 0 12px rgba(255,183,46,.65)":"none"},children:l||u.jsxs("span",{style:{display:"inline-block",minWidth:20},children:[a.position,"."]})}):null,G=i.alignRight?`minmax(0, 1fr)${i.showAvatars?` ${L}px`:""}${i.showMedals?` ${ie}px`:""}`:`${i.showMedals?`${ie}px `:""}${i.showAvatars?`${L}px `:""}minmax(0, 1fr)`;return u.jsxs("div",{className:["lp-rank-row",c?"top1":"",x?"top2":"",y?"top3":"",B?"event-pulse":""].join(" ").trim(),style:{display:"grid",gridTemplateColumns:G,alignItems:"center",columnGap:10,minHeight:i.compactMode?46:54,padding:c?"6px 8px":"4px 6px",borderRadius:14,background:c?w.rowBackgroundTop:w.rowBackground,border:i.theme==="light"?"1px solid rgba(15,23,42,.06)":"1px solid rgba(255,255,255,.045)",position:"relative",transform:B&&c?"scale(1.035)":"scale(1)",transition:"transform 220ms ease, filter 220ms ease"},children:[c?u.jsx("div",{className:J?"top1-halo pulse":"top1-halo",style:{position:"absolute",inset:"2px 2px 2px 2px",borderRadius:16,pointerEvents:"none",background:w.topHalo,filter:"blur(8px)",opacity:.95}}):null,i.alignRight?u.jsxs(u.Fragment,{children:[C,E,A]}):u.jsxs(u.Fragment,{children:[A,E,C]})]},`${a.position}-${a.name}-${a.value}`)})})})}),u.jsx("style",{children:`
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
      `})]})}export{Pe as RanksOverlayPage};
