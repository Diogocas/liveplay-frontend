import{r as g,j as d}from"./index-DCOr3lvJ.js";import{n as ye,c as we,d as W}from"./top-record-widget-rSVRU7kw.js";import{g as ce}from"./app-kv-BJKjXjFm.js";import{r as be}from"./overlay-url-yJYaczpp.js";import"./brazil-gifts-BdHv-gbd.js";const ke="http://127.0.0.1:35942",R={topLike:{id:"topLike",metricIcon:"heart",configKey:"liveplay.toplike.config",snapshotKey:"liveplay.toplike.snapshot",channel:"liveplay-toplike-channel",snapshotType:"top-like-sync",rankKey:"likes",queryBoard:"topLike"},topGifts:{id:"topGifts",metricIcon:"coin",configKey:"liveplay.topgifts.config",snapshotKey:"liveplay.topgifts.snapshot",channel:"liveplay-topgifts-channel",snapshotType:"top-gifts-sync",rankKey:"gifts",queryBoard:"topGifts"},weeklyRank:{id:"weeklyRank",metricIcon:"coin",configKey:"liveplay.weeklyrank.config",snapshotKey:"liveplay.weeklyrank.snapshot",channel:"liveplay-weeklyrank-channel",snapshotType:"weekly-rank-sync",rankKey:"weekly",queryBoard:"weeklyRank"},monthlyRank:{id:"monthlyRank",metricIcon:"coin",configKey:"liveplay.monthlyrank.config",snapshotKey:"liveplay.monthlyrank.snapshot",channel:"liveplay-monthlyrank-channel",snapshotType:"monthly-rank-sync",rankKey:"monthly",queryBoard:"monthlyRank"},topGift:{id:"topGift",metricIcon:"coin",configKey:"liveplay.topgift.config",snapshotKey:"liveplay.topgift.snapshot",channel:"liveplay-topgift-channel",snapshotType:"top-gift-sync",rankKey:"topGift",queryBoard:"topGift"},topCombo:{id:"topCombo",metricIcon:"coin",configKey:"liveplay.topcombo.config",snapshotKey:"liveplay.topcombo.snapshot",channel:"liveplay-topcombo-channel",snapshotType:"top-combo-sync",rankKey:"topCombo",queryBoard:"topCombo"}};function Se(a){return a.trim().replace(/\/$/,"")}function Y(a,e){return`${Se(a)}${e.startsWith("/")?e:`/${e}`}`}const f={theme:"dark",opacity:100,topCount:10,showAvatars:!0,showMedals:!0,showCrown:!0,showMetricIcon:!0,displaySeconds:8,width:820,compactMode:!1,nameFont:"display",valueFont:"display",nameColor:"#fff200",valueColor:"#22d3ee",nameFontSize:100,valueFontSize:100,nameLetterSpacing:0,valueLetterSpacing:0,lineSpacing:100,alignRight:!1},le={display:'"Trebuchet MS", "Arial Black", Impact, sans-serif',default:'Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',tech:'"Lucida Console", "Courier New", monospace',compact:'"Arial Narrow", "Roboto Condensed", "Segoe UI", sans-serif',serif:'Georgia, "Times New Roman", serif'};function q(a){return le[a]||le.default}function pe(a){const e=typeof a=="string"?JSON.parse(a):a??{};return{...f,...e,topCount:e.topCount===3||e.topCount===5||e.topCount===10?e.topCount:f.topCount,theme:e.theme==="light"?"light":"dark",opacity:typeof e.opacity=="number"?Math.max(0,Math.min(100,e.opacity)):f.opacity,displaySeconds:typeof e.displaySeconds=="number"?Math.max(1,Math.min(60,e.displaySeconds)):f.displaySeconds,width:typeof e.width=="number"?Math.max(420,Math.min(1400,e.width)):f.width,nameFont:e.nameFont==="default"||e.nameFont==="display"||e.nameFont==="tech"||e.nameFont==="compact"||e.nameFont==="serif"?e.nameFont:f.nameFont,valueFont:e.valueFont==="default"||e.valueFont==="display"||e.valueFont==="tech"||e.valueFont==="compact"||e.valueFont==="serif"?e.valueFont:f.valueFont,nameColor:typeof e.nameColor=="string"&&e.nameColor.trim()?e.nameColor:f.nameColor,valueColor:typeof e.valueColor=="string"&&e.valueColor.trim()?e.valueColor:f.valueColor,nameFontSize:typeof e.nameFontSize=="number"?Math.max(70,Math.min(180,e.nameFontSize)):f.nameFontSize,valueFontSize:typeof e.valueFontSize=="number"?Math.max(70,Math.min(180,e.valueFontSize)):f.valueFontSize,nameLetterSpacing:typeof e.nameLetterSpacing=="number"?Math.max(-2,Math.min(12,e.nameLetterSpacing)):f.nameLetterSpacing,valueLetterSpacing:typeof e.valueLetterSpacing=="number"?Math.max(-2,Math.min(12,e.valueLetterSpacing)):f.valueLetterSpacing,lineSpacing:typeof e.lineSpacing=="number"?Math.max(70,Math.min(180,e.lineSpacing)):f.lineSpacing,alignRight:typeof e.alignRight=="boolean"?e.alignRight:f.alignRight}}function P(a){try{const e=window.localStorage.getItem(a.configKey);return e?pe(e):f}catch{return f}}function _(a,e){const m=typeof e=="string"?JSON.parse(e):e??{};return{...W(a.id),...m}}function xe(a){try{const e=window.localStorage.getItem(a.configKey);return e?_(a,e):W(a.id)}catch{return W(a.id)}}function H(a){try{const e=window.localStorage.getItem(a.snapshotKey);if(!e)return null;const m=JSON.parse(e);return m.type!==a.snapshotType?null:m}catch{return null}}function Ce(a){return a.id==="topLike"?"likes":a.id==="topGifts"?"gifts":a.id==="weeklyRank"?"weeklyCoins":a.id==="monthlyRank"?"monthlyCoins":a.id==="topGift"?"topGift":"topCombo"}function Re(a){return a==="gold"?"🥇":a==="silver"?"🥈":a==="bronze"?"🥉":null}function Ie(a){return new Intl.NumberFormat("pt-BR").format(a)}function de(a,e,m){var v,b;const n=e==="weekly"?"weeklyCoins":e==="monthly"?"monthlyCoins":e;return(Array.isArray((b=(v=a==null?void 0:a.ranks)==null?void 0:v[n])==null?void 0:b.entries)?a.ranks[n].entries:[]).slice(0,m).map((l,o)=>({position:o+1,name:String((l==null?void 0:l.nickname)||(l==null?void 0:l.username)||`Usuário ${o+1}`),value:Math.max(0,Number((l==null?void 0:l.value)||0)),avatar:String((l==null?void 0:l.avatarUrl)||"").trim()||void 0,medal:o===0?"gold":o===1?"silver":o===2?"bronze":void 0,crowned:o===0,giftId:String((l==null?void 0:l.giftId)||"").trim()||void 0,giftName:String((l==null?void 0:l.giftName)||"").trim()||void 0,giftImageUrl:String((l==null?void 0:l.giftImageUrl)||"").trim()||void 0,comboCount:Math.max(0,Number((l==null?void 0:l.comboCount)||0))||void 0,coinValue:Math.max(0,Number((l==null?void 0:l.coinValue)||0))||void 0}))}function Fe(a,e){return a.map(m=>`${m.position}:${m.name}:${m.value}`).join("|")+`::${e||0}`}function Me(a,e,m,n,T=100,v="display"){return a==="coin"?d.jsx("span",{className:m?"metric-icon metric-coin event-coin":"metric-icon metric-coin",style:{color:n,fontSize:(e?17:15)*(T/100),lineHeight:1,fontFamily:q(v),filter:e?"drop-shadow(0 0 8px rgba(255,207,63,.45))":"none"},children:"●"}):d.jsx("span",{className:m?"metric-icon metric-heart event-heart":"metric-icon metric-heart",style:{color:"#ff2c55",fontSize:e?17:15,lineHeight:1,filter:e?"drop-shadow(0 0 8px rgba(255,44,85,.45))":"none"},children:"♥"})}function Te(){var oe;const a=g.useMemo(()=>new URLSearchParams(window.location.search),[]),e=a.get("board")||"topLike",m=a.get("sourceId")||e,n=e==="topGifts"?R.topGifts:e==="weeklyRank"?R.weeklyRank:e==="monthlyRank"?R.monthlyRank:e==="topGift"?R.topGift:e==="topCombo"?R.topCombo:R.topLike,T=a.get("bridgeUrl"),v=typeof window<"u"&&typeof window.liveplay<"u",[b,l]=g.useState(()=>T||be(a,"/overlay-bridge",ke)),[o,J]=g.useState(()=>n.id==="topGift"||n.id==="topCombo"?xe(n):P(n)),[u,$]=g.useState(()=>H(n)),[ue,Q]=g.useState([]),[fe,X]=g.useState(!1),[V,Z]=g.useState(!1),N=g.useRef((u==null?void 0:u.updatedAt)??0),I=g.useRef(""),D=g.useRef(Number((o==null?void 0:o.updatedAt)||0)),O=g.useRef(!!(o!=null&&o.updatedAt)),F=i=>{if(!i){O.current||J(n.id==="topGift"||n.id==="topCombo"?W(n.id):f);return}const r=n.id==="topGift"||n.id==="topCombo"?_(n,i):pe(i),s=Number((r==null?void 0:r.updatedAt)||0);if(s>0){if(s<D.current)return;D.current=s,O.current=!0}else if(O.current)return;J(r)},K=i=>{var s,c;if(!i||i.type!=="rank-config-sync"||i.board!==n.id)return;const r=Number(i.updatedAt||((s=i.config)==null?void 0:s.updatedAt)||0);r&&r<D.current||F({...i.config,updatedAt:r||Number(((c=i.config)==null?void 0:c.updatedAt)||Date.now())})};g.useEffect(()=>{const i=document.documentElement,r=document.body,s=document.getElementById("root"),c=i.style.background,y=r.style.background,h=(s==null?void 0:s.style.background)??"";return i.style.background="transparent",r.style.background="transparent",s&&(s.style.background="transparent"),()=>{i.style.background=c,r.style.background=y,s&&(s.style.background=h)}},[]),g.useEffect(()=>{var i,r,s;v&&((s=(r=(i=window.liveplay)==null?void 0:i.app)==null?void 0:r.getOverlayBridgeUrl)==null||s.call(r).then(c=>{c!=null&&c.url&&l(c.url)}).catch(()=>{}))},[v]),g.useEffect(()=>{F(P(n));const i=H(n);$(i),N.current=(i==null?void 0:i.updatedAt)??0},[n]),g.useEffect(()=>{let i=!0;return(async()=>{const[s,c]=await Promise.all([ce(n.configKey,null),ce(n.snapshotKey,null)]);i&&(s&&F(s),c&&c.type===n.snapshotType&&(!c.board||c.board===n.id)&&($(c),N.current=c.updatedAt??0))})().catch(()=>{}),()=>{i=!1}},[n]),g.useEffect(()=>{var re,G,se;const i=()=>F(P(n)),r=t=>{if(!t||t.type!==n.snapshotType||typeof t.board=="string"&&t.board&&t.board!==n.id)return;const p=Number(t.updatedAt||t.at||Date.now());p<N.current||(N.current=p,$({...t,updatedAt:p}))},s=t=>{t.key===n.configKey&&i(),t.key===n.snapshotKey&&r(H(n))},c=t=>{r(t.detail)},y=t=>{K(t.detail)};window.addEventListener("storage",s),window.addEventListener(`liveplay:${n.id}-snapshot`,c),window.addEventListener(`liveplay:${n.id}-config`,y);let h=null;try{h=new BroadcastChannel(n.channel),h.onmessage=t=>{const p=t.data;if((p==null?void 0:p.type)==="rank-config-sync"){K(p);return}r(p)}}catch{}const x=window.setInterval(()=>{document.visibilityState==="visible"&&F(P(n))},2500);let k=!1;const j=Ce(n),B=t=>{if(!t||(t==null?void 0:t.type)!=="rank-overlay-sync"||typeof(t==null?void 0:t.board)=="string"&&t.board&&t.board!==j)return;const p=t==null?void 0:t.ranks;p&&(X(!!(t!=null&&t.liveActive)),Q(de({ranks:p},n.rankKey,10)))},z=async()=>{try{const[t,p,L]=await Promise.all([fetch(Y(b,`/snapshot?type=rank-config-sync&board=${n.id}&sourceId=${encodeURIComponent(m)}`),{cache:"no-store"}).then(E=>E.json()).catch(()=>null),fetch(Y(b,`/snapshot?type=${n.snapshotType}&board=${n.id}&sourceId=${encodeURIComponent(m)}`),{cache:"no-store"}).then(E=>E.json()).catch(()=>null),fetch(Y(b,`/snapshot?type=rank-overlay-sync&board=${j}&sourceId=${encodeURIComponent(m)}`),{cache:"no-store"}).then(E=>E.json()).catch(()=>null)]);if(k)return;const w=t==null?void 0:t.payload;(w==null?void 0:w.type)==="rank-config-sync"&&K(w);const C=p==null?void 0:p.payload;(C==null?void 0:C.type)===n.snapshotType&&r({...C,updatedAt:Number(C.updatedAt||C.at||Date.now())});const U=L==null?void 0:L.payload;(U==null?void 0:U.type)==="rank-overlay-sync"&&B(U)}catch{}},ie=(se=(G=(re=window.liveplay)==null?void 0:re.app)==null?void 0:G.onOverlaySync)==null?void 0:se.call(G,t=>{if((t==null?void 0:t.type)==="rank-config-sync"){K(t);return}if((t==null?void 0:t.type)===n.snapshotType){if(typeof(t==null?void 0:t.board)=="string"&&t.board&&t.board!==n.id)return;r({...t,updatedAt:Number(t.updatedAt||t.at||Date.now())});return}(t==null?void 0:t.type)==="rank-overlay-sync"&&B(t)}),ae=async()=>{var t;try{const p=(t=window.liveplay)==null?void 0:t.app;if(!(p!=null&&p.getState)||!(p!=null&&p.getStatus)){await z();return}const[L,w]=await Promise.all([p.getState(),p.getStatus()]);if(k)return;X(!!(w!=null&&w.tiktokConnected)),Q(de(L,n.rankKey,10)),await z()}catch{k||await z()}};ae();const ve=window.setInterval(()=>{document.visibilityState==="visible"&&ae()},900);return()=>{k=!0,window.removeEventListener("storage",s),window.removeEventListener(`liveplay:${n.id}-snapshot`,c),window.removeEventListener(`liveplay:${n.id}-config`,y),window.clearInterval(x),window.clearInterval(ve),h==null||h.close(),typeof ie=="function"&&ie()}},[b,v,n,m]);const ee=n.id==="topGift"||n.id==="topCombo"?1:o.topCount,M=!!(u!=null&&u.visible)&&u.mode==="test"&&((oe=u.entries)==null?void 0:oe.length),S=M?u.entries.slice(0,ee):fe?ue.slice(0,ee):[];if(g.useEffect(()=>{if(!M||!u)return;const i=n.id==="topGift"||n.id==="topCombo"?8:Math.max(1,Math.min(60,Number(o.displaySeconds||f.displaySeconds))),r=window.setTimeout(()=>{$(s=>!s||s.mode!=="test"||s.updatedAt!==u.updatedAt?s:{...s,visible:!1,updatedAt:Date.now()})},i*1e3+450);return()=>window.clearTimeout(r)},[M,u==null?void 0:u.updatedAt,n.id,o]),g.useEffect(()=>{const i=Fe(S,M?u==null?void 0:u.updatedAt:0);if(!i){I.current="";return}if(I.current&&I.current!==i){Z(!0);const r=window.setTimeout(()=>Z(!1),950);return I.current=i,()=>window.clearTimeout(r)}I.current=i},[S,M,u==null?void 0:u.updatedAt]),(n.id==="topGift"||n.id==="topCombo")&&S.length){const i=_(n,o),r=S[0],s=ye({username:r.name,nickname:r.name,avatarUrl:r.avatar,giftId:r.giftId,giftName:r.giftName,giftImageUrl:r.giftImageUrl,comboCount:r.comboCount,coinValue:r.coinValue,value:r.value},n.id);return s?d.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:"flex-start",padding:0,overflow:"hidden"},children:d.jsx("div",{style:{width:390,maxWidth:"100%",padding:10,boxSizing:"border-box"},children:d.jsx(we,{kind:n.id,config:i,entry:s,embedded:!0})})}):d.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}})}if(!S.length)return d.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}});const me=o.compactMode?8:10,A=o.compactMode?42:48,te=o.compactMode?30:34,ne=(o.compactMode?13:15)*(o.nameFontSize/100),ge=(o.compactMode?12:14)*(o.valueFontSize/100),he=2*(o.lineSpacing/100);return d.jsxs(d.Fragment,{children:[d.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:o.alignRight?"flex-end":"flex-start",padding:0,overflow:"hidden"},children:d.jsx("div",{style:{width:Math.min(o.width,1400),minHeight:180,borderRadius:0,border:"none",background:"transparent",padding:o.compactMode?"8px 10px":"10px 12px",boxSizing:"border-box",overflow:"hidden",boxShadow:"none"},children:d.jsx("div",{style:{display:"grid",gap:me,maxWidth:"100%"},children:S.map((i,r)=>{const s=Re(i.medal),c=r===0,y=r===1,h=r===2,x=V&&(c||y||h),k=d.jsxs("div",{style:{minWidth:0,display:"grid",gap:he,position:"relative",zIndex:2,justifyItems:o.alignRight?"end":"start"},children:[d.jsx("div",{style:{color:o.nameColor,fontFamily:q(o.nameFont),fontWeight:c?900:800,fontSize:c?ne+2:ne,lineHeight:1.1,letterSpacing:`${o.nameLetterSpacing}px`,whiteSpace:"nowrap",overflow:"hidden",textOverflow:"ellipsis",textShadow:c?"0 0 10px rgba(255,242,0,.28)":"none"},children:i.name}),d.jsxs("div",{style:{display:"flex",alignItems:"center",gap:6,justifyContent:o.alignRight?"flex-end":"flex-start",color:o.valueColor,fontFamily:q(o.valueFont),fontWeight:800,fontSize:ge,letterSpacing:`${o.valueLetterSpacing}px`},children:[o.showMetricIcon?Me(n.metricIcon,c,x,o.valueColor,o.valueFontSize,o.valueFont):null,d.jsx("span",{className:x?"value-bump":"",style:{display:"inline-block",textShadow:c?"0 0 10px rgba(34,211,238,.22)":"none"},children:Ie(i.value)})]})]}),j=o.showAvatars?d.jsxs("div",{style:{position:"relative",width:A,height:A,borderRadius:"50%",overflow:"visible",border:c?"2px solid rgba(255,215,0,.95)":"2px solid rgba(255,193,7,.75)",background:"rgba(255,255,255,.06)",display:"grid",placeItems:"center",zIndex:2,boxShadow:c?"0 0 18px rgba(255,210,0,.45)":y||h?"0 0 12px rgba(255,255,255,.12)":"none"},children:[i.avatar?d.jsx("img",{src:i.avatar,alt:i.name,style:{width:"100%",height:"100%",objectFit:"cover",borderRadius:"50%"},referrerPolicy:"no-referrer"}):d.jsx("span",{style:{color:"#d1d5db",fontSize:Math.max(18,A*.42),fontWeight:800},children:i.name.charAt(0).toUpperCase()}),o.showCrown&&i.crowned?d.jsx("span",{className:V?"crown crown-bounce":"crown",style:{position:"absolute",top:-18,left:8,fontSize:o.compactMode?22:24,zIndex:3,filter:"drop-shadow(0 0 10px rgba(255,200,0,.55))"},children:"👑"}):null]}):null,B=o.showMedals?d.jsx("div",{style:{color:c?"#ffb72e":i.position<=3?"#ffffff":"#ff3434",fontSize:c?24:i.position<=3?20:32,fontWeight:900,lineHeight:1,textAlign:"center",position:"relative",zIndex:2,textShadow:c?"0 0 12px rgba(255,183,46,.65)":"none"},children:s||d.jsxs("span",{style:{display:"inline-block",minWidth:20},children:[i.position,"."]})}):null,z=o.alignRight?`minmax(0, 1fr)${o.showAvatars?` ${A}px`:""}${o.showMedals?` ${te}px`:""}`:`${o.showMedals?`${te}px `:""}${o.showAvatars?`${A}px `:""}minmax(0, 1fr)`;return d.jsxs("div",{className:["lp-rank-row",c?"top1":"",y?"top2":"",h?"top3":"",x?"event-pulse":""].join(" ").trim(),style:{display:"grid",gridTemplateColumns:z,alignItems:"center",columnGap:10,minHeight:o.compactMode?46:54,padding:c?"6px 8px":"4px 6px",borderRadius:14,background:"transparent",position:"relative",transform:x&&c?"scale(1.035)":"scale(1)",transition:"transform 220ms ease, filter 220ms ease",opacity:Math.max(0,Math.min(1,o.opacity/100))},children:[c?d.jsx("div",{className:V?"top1-halo pulse":"top1-halo",style:{position:"absolute",inset:"2px 2px 2px 2px",borderRadius:16,pointerEvents:"none",background:"radial-gradient(circle at 12% 50%, rgba(255,80,120,.35), transparent 24%), radial-gradient(circle at 72% 50%, rgba(255,195,0,.18), transparent 40%)",filter:"blur(8px)",opacity:.95}}):null,o.alignRight?d.jsxs(d.Fragment,{children:[k,j,B]}):d.jsxs(d.Fragment,{children:[B,j,k]})]},`${i.position}-${i.name}-${i.value}`)})})})}),d.jsx("style",{children:`
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
