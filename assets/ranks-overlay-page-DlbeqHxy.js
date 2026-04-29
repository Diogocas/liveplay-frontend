import{r as f,j as p}from"./index-Qcj6noTV.js";import{n as ve,c as ye,d as P}from"./top-record-widget-DSxmODqB.js";import{g as se}from"./app-kv-BJKjXjFm.js";import{r as we}from"./overlay-url-yJYaczpp.js";import"./brazil-gifts-BdHv-gbd.js";const be="http://127.0.0.1:35942",I={topLike:{id:"topLike",metricIcon:"heart",configKey:"liveplay.toplike.config",snapshotKey:"liveplay.toplike.snapshot",channel:"liveplay-toplike-channel",snapshotType:"top-like-sync",rankKey:"likes",queryBoard:"topLike"},topGifts:{id:"topGifts",metricIcon:"coin",configKey:"liveplay.topgifts.config",snapshotKey:"liveplay.topgifts.snapshot",channel:"liveplay-topgifts-channel",snapshotType:"top-gifts-sync",rankKey:"gifts",queryBoard:"topGifts"},weeklyRank:{id:"weeklyRank",metricIcon:"coin",configKey:"liveplay.weeklyrank.config",snapshotKey:"liveplay.weeklyrank.snapshot",channel:"liveplay-weeklyrank-channel",snapshotType:"weekly-rank-sync",rankKey:"weekly",queryBoard:"weeklyRank"},monthlyRank:{id:"monthlyRank",metricIcon:"coin",configKey:"liveplay.monthlyrank.config",snapshotKey:"liveplay.monthlyrank.snapshot",channel:"liveplay-monthlyrank-channel",snapshotType:"monthly-rank-sync",rankKey:"monthly",queryBoard:"monthlyRank"},topGift:{id:"topGift",metricIcon:"coin",configKey:"liveplay.topgift.config",snapshotKey:"liveplay.topgift.snapshot",channel:"liveplay-topgift-channel",snapshotType:"top-gift-sync",rankKey:"topGift",queryBoard:"topGift"},topCombo:{id:"topCombo",metricIcon:"coin",configKey:"liveplay.topcombo.config",snapshotKey:"liveplay.topcombo.snapshot",channel:"liveplay-topcombo-channel",snapshotType:"top-combo-sync",rankKey:"topCombo",queryBoard:"topCombo"}};function ke(a){return a.trim().replace(/\/$/,"")}function O(a,e){return`${ke(a)}${e.startsWith("/")?e:`/${e}`}`}const m={theme:"dark",opacity:100,topCount:10,showAvatars:!0,showMedals:!0,showCrown:!0,showMetricIcon:!0,displaySeconds:8,width:820,compactMode:!1,nameFont:"display",valueFont:"display",nameColor:"#fff200",valueColor:"#22d3ee",nameFontSize:100,valueFontSize:100,nameLetterSpacing:0,valueLetterSpacing:0,lineSpacing:100,alignRight:!1},ce={display:'"Trebuchet MS", "Arial Black", Impact, sans-serif',default:'Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',tech:'"Lucida Console", "Courier New", monospace',compact:'"Arial Narrow", "Roboto Condensed", "Segoe UI", sans-serif',serif:'Georgia, "Times New Roman", serif'};function H(a){return ce[a]||ce.default}function de(a){const e=typeof a=="string"?JSON.parse(a):a??{};return{...m,...e,topCount:e.topCount===3||e.topCount===5||e.topCount===10?e.topCount:m.topCount,theme:e.theme==="light"?"light":"dark",opacity:typeof e.opacity=="number"?Math.max(0,Math.min(100,e.opacity)):m.opacity,displaySeconds:typeof e.displaySeconds=="number"?Math.max(1,Math.min(60,e.displaySeconds)):m.displaySeconds,width:typeof e.width=="number"?Math.max(420,Math.min(1400,e.width)):m.width,nameFont:e.nameFont==="default"||e.nameFont==="display"||e.nameFont==="tech"||e.nameFont==="compact"||e.nameFont==="serif"?e.nameFont:m.nameFont,valueFont:e.valueFont==="default"||e.valueFont==="display"||e.valueFont==="tech"||e.valueFont==="compact"||e.valueFont==="serif"?e.valueFont:m.valueFont,nameColor:typeof e.nameColor=="string"&&e.nameColor.trim()?e.nameColor:m.nameColor,valueColor:typeof e.valueColor=="string"&&e.valueColor.trim()?e.valueColor:m.valueColor,nameFontSize:typeof e.nameFontSize=="number"?Math.max(70,Math.min(180,e.nameFontSize)):m.nameFontSize,valueFontSize:typeof e.valueFontSize=="number"?Math.max(70,Math.min(180,e.valueFontSize)):m.valueFontSize,nameLetterSpacing:typeof e.nameLetterSpacing=="number"?Math.max(-2,Math.min(12,e.nameLetterSpacing)):m.nameLetterSpacing,valueLetterSpacing:typeof e.valueLetterSpacing=="number"?Math.max(-2,Math.min(12,e.valueLetterSpacing)):m.valueLetterSpacing,lineSpacing:typeof e.lineSpacing=="number"?Math.max(70,Math.min(180,e.lineSpacing)):m.lineSpacing,alignRight:typeof e.alignRight=="boolean"?e.alignRight:m.alignRight}}function U(a){try{const e=window.localStorage.getItem(a.configKey);return e?de(e):m}catch{return m}}function q(a,e){const h=typeof e=="string"?JSON.parse(e):e??{};return{...P(a.id),...h}}function xe(a){try{const e=window.localStorage.getItem(a.configKey);return e?q(a,e):P(a.id)}catch{return P(a.id)}}function Y(a){try{const e=window.localStorage.getItem(a.snapshotKey);if(!e)return null;const h=JSON.parse(e);return h.type!==a.snapshotType?null:h}catch{return null}}function Se(a){return a.id==="topLike"?"likes":a.id==="topGifts"?"gifts":a.id==="weeklyRank"?"weeklyCoins":a.id==="monthlyRank"?"monthlyCoins":a.id==="topGift"?"topGift":"topCombo"}function Ce(a){return a==="gold"?"🥇":a==="silver"?"🥈":a==="bronze"?"🥉":null}function Re(a){return new Intl.NumberFormat("pt-BR").format(a)}function le(a,e,h){var b,x;const t=e==="weekly"?"weeklyCoins":e==="monthly"?"monthlyCoins":e;return(Array.isArray((x=(b=a==null?void 0:a.ranks)==null?void 0:b[t])==null?void 0:x.entries)?a.ranks[t].entries:[]).slice(0,h).map((d,o)=>({position:o+1,name:String((d==null?void 0:d.nickname)||(d==null?void 0:d.username)||`Usuário ${o+1}`),value:Math.max(0,Number((d==null?void 0:d.value)||0)),avatar:String((d==null?void 0:d.avatarUrl)||"").trim()||void 0,medal:o===0?"gold":o===1?"silver":o===2?"bronze":void 0,crowned:o===0,giftId:String((d==null?void 0:d.giftId)||"").trim()||void 0,giftName:String((d==null?void 0:d.giftName)||"").trim()||void 0,giftImageUrl:String((d==null?void 0:d.giftImageUrl)||"").trim()||void 0,comboCount:Math.max(0,Number((d==null?void 0:d.comboCount)||0))||void 0,coinValue:Math.max(0,Number((d==null?void 0:d.coinValue)||0))||void 0}))}function Ie(a){return a.map(e=>`${e.position}:${e.name}:${e.value}`).join("|")}function Me(a,e,h,t,T=100,b="display"){return a==="coin"?p.jsx("span",{className:h?"metric-icon metric-coin event-coin":"metric-icon metric-coin",style:{color:t,fontSize:(e?17:15)*(T/100),lineHeight:1,fontFamily:H(b),filter:e?"drop-shadow(0 0 8px rgba(255,207,63,.45))":"none"},children:"●"}):p.jsx("span",{className:h?"metric-icon metric-heart event-heart":"metric-icon metric-heart",style:{color:"#ff2c55",fontSize:e?17:15,lineHeight:1,filter:e?"drop-shadow(0 0 8px rgba(255,44,85,.45))":"none"},children:"♥"})}function Te(){var ne;const a=f.useMemo(()=>new URLSearchParams(window.location.search),[]),e=a.get("board")||"topLike",h=a.get("sourceId")||e,t=e==="topGifts"?I.topGifts:e==="weeklyRank"?I.weeklyRank:e==="monthlyRank"?I.monthlyRank:e==="topGift"?I.topGift:e==="topCombo"?I.topCombo:I.topLike,T=a.get("bridgeUrl"),b=typeof window<"u"&&typeof window.liveplay<"u",[x,d]=f.useState(()=>T||we(a,"/overlay-bridge",be)),[o,_]=f.useState(()=>t.id==="topGift"||t.id==="topCombo"?xe(t):U(t)),[c,E]=f.useState(()=>Y(t)),[pe,J]=f.useState([]),[ue,Q]=f.useState(!1),[W,X]=f.useState(!1),S=f.useRef((c==null?void 0:c.updatedAt)??0),M=f.useRef(""),D=f.useRef(Number((o==null?void 0:o.updatedAt)||0)),V=f.useRef(!!(o!=null&&o.updatedAt)),F=i=>{if(!i){V.current||_(t.id==="topGift"||t.id==="topCombo"?P(t.id):m);return}const r=t.id==="topGift"||t.id==="topCombo"?q(t,i):de(i),l=Number((r==null?void 0:r.updatedAt)||0);if(l>0){if(l<D.current)return;D.current=l,V.current=!0}else if(V.current)return;_(r)},N=i=>{var l,s;if(!i||i.type!=="rank-config-sync"||i.board!==t.id)return;const r=Number(i.updatedAt||((l=i.config)==null?void 0:l.updatedAt)||0);r&&r<D.current||F({...i.config,updatedAt:r||Number(((s=i.config)==null?void 0:s.updatedAt)||Date.now())})};f.useEffect(()=>{const i=document.documentElement,r=document.body,l=document.getElementById("root"),s=i.style.background,y=r.style.background,v=(l==null?void 0:l.style.background)??"";return i.style.background="transparent",r.style.background="transparent",l&&(l.style.background="transparent"),()=>{i.style.background=s,r.style.background=y,l&&(l.style.background=v)}},[]),f.useEffect(()=>{var i,r,l;b&&((l=(r=(i=window.liveplay)==null?void 0:i.app)==null?void 0:r.getOverlayBridgeUrl)==null||l.call(r).then(s=>{s!=null&&s.url&&d(s.url)}).catch(()=>{}))},[b]),f.useEffect(()=>{F(U(t));const i=Y(t);E(i),S.current=(i==null?void 0:i.updatedAt)??0},[t]),f.useEffect(()=>{let i=!0;return(async()=>{const[l,s]=await Promise.all([se(t.configKey,null),se(t.snapshotKey,null)]);i&&(l&&F(l),s&&s.type===t.snapshotType&&(!s.board||s.board===t.id)&&(E(s),S.current=s.updatedAt??0))})().catch(()=>{}),()=>{i=!1}},[t]),f.useEffect(()=>{var ae,K,re;const i=()=>F(U(t)),r=n=>{if(!n||n.type!==t.snapshotType||typeof n.board=="string"&&n.board&&n.board!==t.id)return;const u=Number(n.updatedAt||n.at||Date.now());u<S.current||(S.current=u,E({...n,updatedAt:u}))},l=n=>{n.key===t.configKey&&i(),n.key===t.snapshotKey&&r(Y(t))},s=n=>{r(n.detail)},y=n=>{N(n.detail)};window.addEventListener("storage",l),window.addEventListener(`liveplay:${t.id}-snapshot`,s),window.addEventListener(`liveplay:${t.id}-config`,y);let v=null;try{v=new BroadcastChannel(t.channel),v.onmessage=n=>{const u=n.data;if((u==null?void 0:u.type)==="rank-config-sync"){N(u);return}r(u)}}catch{}const g=window.setInterval(()=>{document.visibilityState==="visible"&&F(U(t))},2500);let w=!1;const j=Se(t),B=n=>{if(!n||(n==null?void 0:n.type)!=="rank-overlay-sync"||typeof(n==null?void 0:n.board)=="string"&&n.board&&n.board!==j)return;const u=n==null?void 0:n.ranks;u&&(Q(!!(n!=null&&n.liveActive)),J(le({ranks:u},t.rankKey,10)))},$=async()=>{try{const[n,u,z]=await Promise.all([fetch(O(x,`/snapshot?type=rank-config-sync&board=${t.id}&sourceId=${encodeURIComponent(h)}`),{cache:"no-store"}).then(L=>L.json()).catch(()=>null),fetch(O(x,`/snapshot?type=${t.snapshotType}&board=${t.id}&sourceId=${encodeURIComponent(h)}`),{cache:"no-store"}).then(L=>L.json()).catch(()=>null),fetch(O(x,`/snapshot?type=rank-overlay-sync&board=${j}&sourceId=${encodeURIComponent(h)}`),{cache:"no-store"}).then(L=>L.json()).catch(()=>null)]);if(w)return;const k=n==null?void 0:n.payload;(k==null?void 0:k.type)==="rank-config-sync"&&N(k);const R=u==null?void 0:u.payload;(R==null?void 0:R.type)===t.snapshotType&&r({...R,updatedAt:Number(R.updatedAt||R.at||Date.now())});const G=z==null?void 0:z.payload;(G==null?void 0:G.type)==="rank-overlay-sync"&&B(G)}catch{}},oe=(re=(K=(ae=window.liveplay)==null?void 0:ae.app)==null?void 0:K.onOverlaySync)==null?void 0:re.call(K,n=>{if((n==null?void 0:n.type)==="rank-config-sync"){N(n);return}if((n==null?void 0:n.type)===t.snapshotType){if(typeof(n==null?void 0:n.board)=="string"&&n.board&&n.board!==t.id)return;r({...n,updatedAt:Number(n.updatedAt||n.at||Date.now())});return}(n==null?void 0:n.type)==="rank-overlay-sync"&&B(n)}),ie=async()=>{var n;try{const u=(n=window.liveplay)==null?void 0:n.app;if(!(u!=null&&u.getState)||!(u!=null&&u.getStatus)){await $();return}const[z,k]=await Promise.all([u.getState(),u.getStatus()]);if(w)return;Q(!!(k!=null&&k.tiktokConnected)),J(le(z,t.rankKey,10)),await $()}catch{w||await $()}};ie();const he=window.setInterval(()=>{document.visibilityState==="visible"&&ie()},900);return()=>{w=!0,window.removeEventListener("storage",l),window.removeEventListener(`liveplay:${t.id}-snapshot`,s),window.removeEventListener(`liveplay:${t.id}-config`,y),window.clearInterval(g),window.clearInterval(he),v==null||v.close(),typeof oe=="function"&&oe()}},[x,b,t,h]),f.useEffect(()=>{if(!(c!=null&&c.visible)||c.mode!=="test")return;const i=t.id==="topGift"||t.id==="topCombo"?8e3:Math.max(1,Number(o.displaySeconds||m.displaySeconds))*1e3,r=Number(c.updatedAt||c.at||Date.now()),l=Math.max(0,Date.now()-r),s=Math.max(250,i-l+250),y=`${c.type}:${c.board||t.id}:${c.updatedAt}`,v=window.setTimeout(()=>{E(g=>{const w=g?`${g.type}:${g.board||t.id}:${g.updatedAt}`:"";return!(g!=null&&g.visible)||g.mode!=="test"||w!==y?g:(S.current=Math.max(S.current+1,Date.now()),{...g,visible:!1,updatedAt:S.current})})},s);return()=>window.clearTimeout(v)},[c==null?void 0:c.type,c==null?void 0:c.board,c==null?void 0:c.updatedAt,c==null?void 0:c.visible,c==null?void 0:c.mode,t.id,o]);const Z=t.id==="topGift"||t.id==="topCombo"?1:o.topCount,C=!!(c!=null&&c.visible)&&c.mode==="test"&&((ne=c.entries)==null?void 0:ne.length)?c.entries.slice(0,Z):ue?pe.slice(0,Z):[];if(f.useEffect(()=>{const i=Ie(C);if(!i){M.current="";return}if(M.current&&M.current!==i){X(!0);const r=window.setTimeout(()=>X(!1),950);return M.current=i,()=>window.clearTimeout(r)}M.current=i},[C]),(t.id==="topGift"||t.id==="topCombo")&&C.length){const i=q(t,o),r=C[0],l=ve({username:r.name,nickname:r.name,avatarUrl:r.avatar,giftId:r.giftId,giftName:r.giftName,giftImageUrl:r.giftImageUrl,comboCount:r.comboCount,coinValue:r.coinValue,value:r.value},t.id);return l?p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:"flex-start",padding:0,overflow:"hidden"},children:p.jsx("div",{style:{width:390,maxWidth:"100%",padding:10,boxSizing:"border-box"},children:p.jsx(ye,{kind:t.id,config:i,entry:l,embedded:!0})})}):p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}})}if(!C.length)return p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}});const me=o.compactMode?8:10,A=o.compactMode?42:48,ee=o.compactMode?30:34,te=(o.compactMode?13:15)*(o.nameFontSize/100),fe=(o.compactMode?12:14)*(o.valueFontSize/100),ge=2*(o.lineSpacing/100);return p.jsxs(p.Fragment,{children:[p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:o.alignRight?"flex-end":"flex-start",padding:0,overflow:"hidden"},children:p.jsx("div",{style:{width:Math.min(o.width,1400),minHeight:180,borderRadius:0,border:"none",background:"transparent",padding:o.compactMode?"8px 10px":"10px 12px",boxSizing:"border-box",overflow:"hidden",boxShadow:"none"},children:p.jsx("div",{style:{display:"grid",gap:me,maxWidth:"100%"},children:C.map((i,r)=>{const l=Ce(i.medal),s=r===0,y=r===1,v=r===2,g=W&&(s||y||v),w=p.jsxs("div",{style:{minWidth:0,display:"grid",gap:ge,position:"relative",zIndex:2,justifyItems:o.alignRight?"end":"start"},children:[p.jsx("div",{style:{color:o.nameColor,fontFamily:H(o.nameFont),fontWeight:s?900:800,fontSize:s?te+2:te,lineHeight:1.1,letterSpacing:`${o.nameLetterSpacing}px`,whiteSpace:"nowrap",overflow:"hidden",textOverflow:"ellipsis",textShadow:s?"0 0 10px rgba(255,242,0,.28)":"none"},children:i.name}),p.jsxs("div",{style:{display:"flex",alignItems:"center",gap:6,justifyContent:o.alignRight?"flex-end":"flex-start",color:o.valueColor,fontFamily:H(o.valueFont),fontWeight:800,fontSize:fe,letterSpacing:`${o.valueLetterSpacing}px`},children:[o.showMetricIcon?Me(t.metricIcon,s,g,o.valueColor,o.valueFontSize,o.valueFont):null,p.jsx("span",{className:g?"value-bump":"",style:{display:"inline-block",textShadow:s?"0 0 10px rgba(34,211,238,.22)":"none"},children:Re(i.value)})]})]}),j=o.showAvatars?p.jsxs("div",{style:{position:"relative",width:A,height:A,borderRadius:"50%",overflow:"visible",border:s?"2px solid rgba(255,215,0,.95)":"2px solid rgba(255,193,7,.75)",background:"rgba(255,255,255,.06)",display:"grid",placeItems:"center",zIndex:2,boxShadow:s?"0 0 18px rgba(255,210,0,.45)":y||v?"0 0 12px rgba(255,255,255,.12)":"none"},children:[i.avatar?p.jsx("img",{src:i.avatar,alt:i.name,style:{width:"100%",height:"100%",objectFit:"cover",borderRadius:"50%"},referrerPolicy:"no-referrer"}):p.jsx("span",{style:{color:"#d1d5db",fontSize:Math.max(18,A*.42),fontWeight:800},children:i.name.charAt(0).toUpperCase()}),o.showCrown&&i.crowned?p.jsx("span",{className:W?"crown crown-bounce":"crown",style:{position:"absolute",top:-18,left:8,fontSize:o.compactMode?22:24,zIndex:3,filter:"drop-shadow(0 0 10px rgba(255,200,0,.55))"},children:"👑"}):null]}):null,B=o.showMedals?p.jsx("div",{style:{color:s?"#ffb72e":i.position<=3?"#ffffff":"#ff3434",fontSize:s?24:i.position<=3?20:32,fontWeight:900,lineHeight:1,textAlign:"center",position:"relative",zIndex:2,textShadow:s?"0 0 12px rgba(255,183,46,.65)":"none"},children:l||p.jsxs("span",{style:{display:"inline-block",minWidth:20},children:[i.position,"."]})}):null,$=o.alignRight?`minmax(0, 1fr)${o.showAvatars?` ${A}px`:""}${o.showMedals?` ${ee}px`:""}`:`${o.showMedals?`${ee}px `:""}${o.showAvatars?`${A}px `:""}minmax(0, 1fr)`;return p.jsxs("div",{className:["lp-rank-row",s?"top1":"",y?"top2":"",v?"top3":"",g?"event-pulse":""].join(" ").trim(),style:{display:"grid",gridTemplateColumns:$,alignItems:"center",columnGap:10,minHeight:o.compactMode?46:54,padding:s?"6px 8px":"4px 6px",borderRadius:14,background:"transparent",position:"relative",transform:g&&s?"scale(1.035)":"scale(1)",transition:"transform 220ms ease, filter 220ms ease",opacity:Math.max(0,Math.min(1,o.opacity/100))},children:[s?p.jsx("div",{className:W?"top1-halo pulse":"top1-halo",style:{position:"absolute",inset:"2px 2px 2px 2px",borderRadius:16,pointerEvents:"none",background:"radial-gradient(circle at 12% 50%, rgba(255,80,120,.35), transparent 24%), radial-gradient(circle at 72% 50%, rgba(255,195,0,.18), transparent 40%)",filter:"blur(8px)",opacity:.95}}):null,o.alignRight?p.jsxs(p.Fragment,{children:[w,j,B]}):p.jsxs(p.Fragment,{children:[B,j,w]})]},`${i.position}-${i.name}-${i.value}`)})})})}),p.jsx("style",{children:`
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
