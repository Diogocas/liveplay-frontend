import{r as f,j as p}from"./index-CLKMUagm.js";import{n as ve,c as ye,d as U}from"./top-record-widget-DFbq0qlk.js";import{g as se}from"./app-kv-BJKjXjFm.js";import{r as we}from"./overlay-url-yJYaczpp.js";import"./brazil-gifts-BdHv-gbd.js";const be="http://127.0.0.1:35942",R={topLike:{id:"topLike",metricIcon:"heart",configKey:"liveplay.toplike.config",snapshotKey:"liveplay.toplike.snapshot",channel:"liveplay-toplike-channel",snapshotType:"top-like-sync",rankKey:"likes",queryBoard:"topLike"},topGifts:{id:"topGifts",metricIcon:"coin",configKey:"liveplay.topgifts.config",snapshotKey:"liveplay.topgifts.snapshot",channel:"liveplay-topgifts-channel",snapshotType:"top-gifts-sync",rankKey:"gifts",queryBoard:"topGifts"},weeklyRank:{id:"weeklyRank",metricIcon:"coin",configKey:"liveplay.weeklyrank.config",snapshotKey:"liveplay.weeklyrank.snapshot",channel:"liveplay-weeklyrank-channel",snapshotType:"weekly-rank-sync",rankKey:"weekly",queryBoard:"weeklyRank"},monthlyRank:{id:"monthlyRank",metricIcon:"coin",configKey:"liveplay.monthlyrank.config",snapshotKey:"liveplay.monthlyrank.snapshot",channel:"liveplay-monthlyrank-channel",snapshotType:"monthly-rank-sync",rankKey:"monthly",queryBoard:"monthlyRank"},topGift:{id:"topGift",metricIcon:"coin",configKey:"liveplay.topgift.config",snapshotKey:"liveplay.topgift.snapshot",channel:"liveplay-topgift-channel",snapshotType:"top-gift-sync",rankKey:"topGift",queryBoard:"topGift"},topCombo:{id:"topCombo",metricIcon:"coin",configKey:"liveplay.topcombo.config",snapshotKey:"liveplay.topcombo.snapshot",channel:"liveplay-topcombo-channel",snapshotType:"top-combo-sync",rankKey:"topCombo",queryBoard:"topCombo"}};function ke(r){return r.trim().replace(/\/$/,"")}function Y(r,e){return`${ke(r)}${e.startsWith("/")?e:`/${e}`}`}const u={theme:"dark",opacity:100,topCount:10,showAvatars:!0,showMedals:!0,showCrown:!0,showMetricIcon:!0,displaySeconds:8,width:820,compactMode:!1,nameFont:"display",valueFont:"display",nameColor:"#fff200",valueColor:"#22d3ee",nameFontSize:100,valueFontSize:100,nameLetterSpacing:0,valueLetterSpacing:0,lineSpacing:100,alignRight:!1},ce={display:'"Trebuchet MS", "Arial Black", Impact, sans-serif',default:'Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',tech:'"Lucida Console", "Courier New", monospace',compact:'"Arial Narrow", "Roboto Condensed", "Segoe UI", sans-serif',serif:'Georgia, "Times New Roman", serif'};function q(r){return ce[r]||ce.default}function pe(r){const e=typeof r=="string"?JSON.parse(r):r??{};return{...u,...e,topCount:e.topCount===3||e.topCount===5||e.topCount===10?e.topCount:u.topCount,theme:e.theme==="light"?"light":"dark",opacity:typeof e.opacity=="number"?Math.max(0,Math.min(100,e.opacity)):u.opacity,displaySeconds:typeof e.displaySeconds=="number"?Math.max(1,Math.min(60,e.displaySeconds)):u.displaySeconds,width:typeof e.width=="number"?Math.max(420,Math.min(1400,e.width)):u.width,nameFont:e.nameFont==="default"||e.nameFont==="display"||e.nameFont==="tech"||e.nameFont==="compact"||e.nameFont==="serif"?e.nameFont:u.nameFont,valueFont:e.valueFont==="default"||e.valueFont==="display"||e.valueFont==="tech"||e.valueFont==="compact"||e.valueFont==="serif"?e.valueFont:u.valueFont,nameColor:typeof e.nameColor=="string"&&e.nameColor.trim()?e.nameColor:u.nameColor,valueColor:typeof e.valueColor=="string"&&e.valueColor.trim()?e.valueColor:u.valueColor,nameFontSize:typeof e.nameFontSize=="number"?Math.max(70,Math.min(180,e.nameFontSize)):u.nameFontSize,valueFontSize:typeof e.valueFontSize=="number"?Math.max(70,Math.min(180,e.valueFontSize)):u.valueFontSize,nameLetterSpacing:typeof e.nameLetterSpacing=="number"?Math.max(-2,Math.min(12,e.nameLetterSpacing)):u.nameLetterSpacing,valueLetterSpacing:typeof e.valueLetterSpacing=="number"?Math.max(-2,Math.min(12,e.valueLetterSpacing)):u.valueLetterSpacing,lineSpacing:typeof e.lineSpacing=="number"?Math.max(70,Math.min(180,e.lineSpacing)):u.lineSpacing,alignRight:typeof e.alignRight=="boolean"?e.alignRight:u.alignRight}}function G(r){try{const e=window.localStorage.getItem(r.configKey);return e?pe(e):u}catch{return u}}function D(r,e){const m=typeof e=="string"?JSON.parse(e):e??{};return{...U(r.id),...m}}function Se(r){try{const e=window.localStorage.getItem(r.configKey);return e?D(r,e):U(r.id)}catch{return U(r.id)}}function H(r){try{const e=window.localStorage.getItem(r.snapshotKey);if(!e)return null;const m=JSON.parse(e);return m.type!==r.snapshotType?null:m}catch{return null}}function xe(r){return r.id==="topLike"?"likes":r.id==="topGifts"?"gifts":r.id==="weeklyRank"?"weeklyCoins":r.id==="monthlyRank"?"monthlyCoins":r.id==="topGift"?"topGift":"topCombo"}function Ce(r){return r==="gold"?"🥇":r==="silver"?"🥈":r==="bronze"?"🥉":null}function Re(r){return new Intl.NumberFormat("pt-BR").format(r)}function le(r,e,m){var v,b;const n=e==="weekly"?"weeklyCoins":e==="monthly"?"monthlyCoins":e;return(Array.isArray((b=(v=r==null?void 0:r.ranks)==null?void 0:v[n])==null?void 0:b.entries)?r.ranks[n].entries:[]).slice(0,m).map((c,o)=>({position:o+1,name:String((c==null?void 0:c.nickname)||(c==null?void 0:c.username)||`Usuário ${o+1}`),value:Math.max(0,Number((c==null?void 0:c.value)||0)),avatar:String((c==null?void 0:c.avatarUrl)||"").trim()||void 0,medal:o===0?"gold":o===1?"silver":o===2?"bronze":void 0,crowned:o===0,giftId:String((c==null?void 0:c.giftId)||"").trim()||void 0,giftName:String((c==null?void 0:c.giftName)||"").trim()||void 0,giftImageUrl:String((c==null?void 0:c.giftImageUrl)||"").trim()||void 0,comboCount:Math.max(0,Number((c==null?void 0:c.comboCount)||0))||void 0,coinValue:Math.max(0,Number((c==null?void 0:c.coinValue)||0))||void 0}))}function Ie(r){return r.map(e=>`${e.position}:${e.name}:${e.value}`).join("|")}function Fe(r,e,m,n,E=100,v="display"){return r==="coin"?p.jsx("span",{className:m?"metric-icon metric-coin event-coin":"metric-icon metric-coin",style:{color:n,fontSize:(e?17:15)*(E/100),lineHeight:1,fontFamily:q(v),filter:e?"drop-shadow(0 0 8px rgba(255,207,63,.45))":"none"},children:"●"}):p.jsx("span",{className:m?"metric-icon metric-heart event-heart":"metric-icon metric-heart",style:{color:"#ff2c55",fontSize:e?17:15,lineHeight:1,filter:e?"drop-shadow(0 0 8px rgba(255,44,85,.45))":"none"},children:"♥"})}function Te(){var ne;const r=f.useMemo(()=>new URLSearchParams(window.location.search),[]),e=r.get("board")||"topLike",m=r.get("sourceId")||e,n=e==="topGifts"?R.topGifts:e==="weeklyRank"?R.weeklyRank:e==="monthlyRank"?R.monthlyRank:e==="topGift"?R.topGift:e==="topCombo"?R.topCombo:R.topLike,E=r.get("bridgeUrl"),v=typeof window<"u"&&typeof window.liveplay<"u",[b,c]=f.useState(()=>E||we(r,"/overlay-bridge",be)),[o,_]=f.useState(()=>n.id==="topGift"||n.id==="topCombo"?Se(n):G(n)),[g,P]=f.useState(()=>H(n)),[de,J]=f.useState([]),[ue,Q]=f.useState(!1),[W,X]=f.useState(!1),T=f.useRef((g==null?void 0:g.updatedAt)??0),I=f.useRef(""),V=f.useRef(Number((o==null?void 0:o.updatedAt)||0)),O=f.useRef(!!(o!=null&&o.updatedAt)),F=i=>{if(!i){O.current||_(n.id==="topGift"||n.id==="topCombo"?U(n.id):u);return}const a=n.id==="topGift"||n.id==="topCombo"?D(n,i):pe(i),l=Number((a==null?void 0:a.updatedAt)||0);if(l>0){if(l<V.current)return;V.current=l,O.current=!0}else if(O.current)return;_(a)},$=i=>{var l,s;if(!i||i.type!=="rank-config-sync"||i.board!==n.id)return;const a=Number(i.updatedAt||((l=i.config)==null?void 0:l.updatedAt)||0);a&&a<V.current||F({...i.config,updatedAt:a||Number(((s=i.config)==null?void 0:s.updatedAt)||Date.now())})};f.useEffect(()=>{const i=document.documentElement,a=document.body,l=document.getElementById("root"),s=i.style.background,y=a.style.background,h=(l==null?void 0:l.style.background)??"";return i.style.background="transparent",a.style.background="transparent",l&&(l.style.background="transparent"),()=>{i.style.background=s,a.style.background=y,l&&(l.style.background=h)}},[]),f.useEffect(()=>{var i,a,l;v&&((l=(a=(i=window.liveplay)==null?void 0:i.app)==null?void 0:a.getOverlayBridgeUrl)==null||l.call(a).then(s=>{s!=null&&s.url&&c(s.url)}).catch(()=>{}))},[v]),f.useEffect(()=>{F(G(n));const i=H(n);P(i),T.current=(i==null?void 0:i.updatedAt)??0},[n]),f.useEffect(()=>{let i=!0;return(async()=>{const[l,s]=await Promise.all([se(n.configKey,null),se(n.snapshotKey,null)]);i&&(l&&F(l),s&&s.type===n.snapshotType&&(!s.board||s.board===n.id)&&(P(s),T.current=s.updatedAt??0))})().catch(()=>{}),()=>{i=!1}},[n]),f.useEffect(()=>{var re,N,ae;const i=()=>F(G(n)),a=t=>{if(!t||t.type!==n.snapshotType||typeof t.board=="string"&&t.board&&t.board!==n.id)return;const d=Number(t.updatedAt||t.at||0);d<T.current||(T.current=d,P({...t,updatedAt:d}))},l=t=>{t.key===n.configKey&&i(),t.key===n.snapshotKey&&a(H(n))},s=t=>{a(t.detail)},y=t=>{$(t.detail)};window.addEventListener("storage",l),window.addEventListener(`liveplay:${n.id}-snapshot`,s),window.addEventListener(`liveplay:${n.id}-config`,y);let h=null;try{h=new BroadcastChannel(n.channel),h.onmessage=t=>{const d=t.data;if((d==null?void 0:d.type)==="rank-config-sync"){$(d);return}a(d)}}catch{}const x=window.setInterval(()=>{document.visibilityState==="visible"&&F(G(n))},2500);let k=!1;const j=xe(n),A=t=>{if(!t||(t==null?void 0:t.type)!=="rank-overlay-sync"||typeof(t==null?void 0:t.board)=="string"&&t.board&&t.board!==j)return;const d=t==null?void 0:t.ranks;d&&(Q(!!(t!=null&&t.liveActive)),J(le({ranks:d},n.rankKey,10)))},B=async()=>{try{const[t,d,z]=await Promise.all([fetch(Y(b,`/snapshot?type=rank-config-sync&board=${n.id}&sourceId=${encodeURIComponent(m)}`),{cache:"no-store"}).then(L=>L.json()).catch(()=>null),fetch(Y(b,`/snapshot?type=${n.snapshotType}&board=${n.id}&sourceId=${encodeURIComponent(m)}`),{cache:"no-store"}).then(L=>L.json()).catch(()=>null),fetch(Y(b,`/snapshot?type=rank-overlay-sync&board=${j}&sourceId=${encodeURIComponent(m)}`),{cache:"no-store"}).then(L=>L.json()).catch(()=>null)]);if(k)return;const w=t==null?void 0:t.payload;(w==null?void 0:w.type)==="rank-config-sync"&&$(w);const C=d==null?void 0:d.payload;(C==null?void 0:C.type)===n.snapshotType&&a({...C,updatedAt:Number(C.updatedAt||C.at||Date.now())});const K=z==null?void 0:z.payload;(K==null?void 0:K.type)==="rank-overlay-sync"&&A(K)}catch{}},oe=(ae=(N=(re=window.liveplay)==null?void 0:re.app)==null?void 0:N.onOverlaySync)==null?void 0:ae.call(N,t=>{if((t==null?void 0:t.type)==="rank-config-sync"){$(t);return}if((t==null?void 0:t.type)===n.snapshotType){if(typeof(t==null?void 0:t.board)=="string"&&t.board&&t.board!==n.id)return;a({...t,updatedAt:Number(t.updatedAt||t.at||Date.now())});return}(t==null?void 0:t.type)==="rank-overlay-sync"&&A(t)}),ie=async()=>{var t;try{const d=(t=window.liveplay)==null?void 0:t.app;if(!(d!=null&&d.getState)||!(d!=null&&d.getStatus)){await B();return}const[z,w]=await Promise.all([d.getState(),d.getStatus()]);if(k)return;Q(!!(w!=null&&w.tiktokConnected)),J(le(z,n.rankKey,10)),await B()}catch{k||await B()}};ie();const ge=window.setInterval(()=>{document.visibilityState==="visible"&&ie()},900);return()=>{k=!0,window.removeEventListener("storage",l),window.removeEventListener(`liveplay:${n.id}-snapshot`,s),window.removeEventListener(`liveplay:${n.id}-config`,y),window.clearInterval(x),window.clearInterval(ge),h==null||h.close(),typeof oe=="function"&&oe()}},[b,v,n,m]);const Z=n.id==="topGift"||n.id==="topCombo"?1:o.topCount,S=!!(g!=null&&g.visible)&&g.mode==="test"&&((ne=g.entries)==null?void 0:ne.length)?g.entries.slice(0,Z):ue?de.slice(0,Z):[];if(f.useEffect(()=>{const i=Ie(S);if(!i){I.current="";return}if(I.current&&I.current!==i){X(!0);const a=window.setTimeout(()=>X(!1),950);return I.current=i,()=>window.clearTimeout(a)}I.current=i},[S]),(n.id==="topGift"||n.id==="topCombo")&&S.length){const i=D(n,o),a=S[0],l=ve({username:a.name,nickname:a.name,avatarUrl:a.avatar,giftId:a.giftId,giftName:a.giftName,giftImageUrl:a.giftImageUrl,comboCount:a.comboCount,coinValue:a.coinValue,value:a.value},n.id);return l?p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:"flex-start",padding:0,overflow:"hidden"},children:p.jsx("div",{style:{width:390,maxWidth:"100%",padding:10,boxSizing:"border-box"},children:p.jsx(ye,{kind:n.id,config:i,entry:l,embedded:!0})})}):p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}})}if(!S.length)return p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent"}});const fe=o.compactMode?8:10,M=o.compactMode?42:48,ee=o.compactMode?30:34,te=(o.compactMode?13:15)*(o.nameFontSize/100),me=(o.compactMode?12:14)*(o.valueFontSize/100),he=2*(o.lineSpacing/100);return p.jsxs(p.Fragment,{children:[p.jsx("div",{style:{width:"100%",height:"100%",background:"transparent",display:"flex",alignItems:"flex-start",justifyContent:o.alignRight?"flex-end":"flex-start",padding:0,overflow:"hidden"},children:p.jsx("div",{style:{width:Math.min(o.width,1400),minHeight:180,borderRadius:0,border:"none",background:"transparent",padding:o.compactMode?"8px 10px":"10px 12px",boxSizing:"border-box",overflow:"hidden",boxShadow:"none"},children:p.jsx("div",{style:{display:"grid",gap:fe,maxWidth:"100%"},children:S.map((i,a)=>{const l=Ce(i.medal),s=a===0,y=a===1,h=a===2,x=W&&(s||y||h),k=p.jsxs("div",{style:{minWidth:0,display:"grid",gap:he,position:"relative",zIndex:2,justifyItems:o.alignRight?"end":"start"},children:[p.jsx("div",{style:{color:o.nameColor,fontFamily:q(o.nameFont),fontWeight:s?900:800,fontSize:s?te+2:te,lineHeight:1.1,letterSpacing:`${o.nameLetterSpacing}px`,whiteSpace:"nowrap",overflow:"hidden",textOverflow:"ellipsis",textShadow:s?"0 0 10px rgba(255,242,0,.28)":"none"},children:i.name}),p.jsxs("div",{style:{display:"flex",alignItems:"center",gap:6,justifyContent:o.alignRight?"flex-end":"flex-start",color:o.valueColor,fontFamily:q(o.valueFont),fontWeight:800,fontSize:me,letterSpacing:`${o.valueLetterSpacing}px`},children:[o.showMetricIcon?Fe(n.metricIcon,s,x,o.valueColor,o.valueFontSize,o.valueFont):null,p.jsx("span",{className:x?"value-bump":"",style:{display:"inline-block",textShadow:s?"0 0 10px rgba(34,211,238,.22)":"none"},children:Re(i.value)})]})]}),j=o.showAvatars?p.jsxs("div",{style:{position:"relative",width:M,height:M,borderRadius:"50%",overflow:"visible",border:s?"2px solid rgba(255,215,0,.95)":"2px solid rgba(255,193,7,.75)",background:"rgba(255,255,255,.06)",display:"grid",placeItems:"center",zIndex:2,boxShadow:s?"0 0 18px rgba(255,210,0,.45)":y||h?"0 0 12px rgba(255,255,255,.12)":"none"},children:[i.avatar?p.jsx("img",{src:i.avatar,alt:i.name,style:{width:"100%",height:"100%",objectFit:"cover",borderRadius:"50%"},referrerPolicy:"no-referrer"}):p.jsx("span",{style:{color:"#d1d5db",fontSize:Math.max(18,M*.42),fontWeight:800},children:i.name.charAt(0).toUpperCase()}),o.showCrown&&i.crowned?p.jsx("span",{className:W?"crown crown-bounce":"crown",style:{position:"absolute",top:-18,left:8,fontSize:o.compactMode?22:24,zIndex:3,filter:"drop-shadow(0 0 10px rgba(255,200,0,.55))"},children:"👑"}):null]}):null,A=o.showMedals?p.jsx("div",{style:{color:s?"#ffb72e":i.position<=3?"#ffffff":"#ff3434",fontSize:s?24:i.position<=3?20:32,fontWeight:900,lineHeight:1,textAlign:"center",position:"relative",zIndex:2,textShadow:s?"0 0 12px rgba(255,183,46,.65)":"none"},children:l||p.jsxs("span",{style:{display:"inline-block",minWidth:20},children:[i.position,"."]})}):null,B=o.alignRight?`minmax(0, 1fr)${o.showAvatars?` ${M}px`:""}${o.showMedals?` ${ee}px`:""}`:`${o.showMedals?`${ee}px `:""}${o.showAvatars?`${M}px `:""}minmax(0, 1fr)`;return p.jsxs("div",{className:["lp-rank-row",s?"top1":"",y?"top2":"",h?"top3":"",x?"event-pulse":""].join(" ").trim(),style:{display:"grid",gridTemplateColumns:B,alignItems:"center",columnGap:10,minHeight:o.compactMode?46:54,padding:s?"6px 8px":"4px 6px",borderRadius:14,background:"transparent",position:"relative",transform:x&&s?"scale(1.035)":"scale(1)",transition:"transform 220ms ease, filter 220ms ease",opacity:Math.max(0,Math.min(1,o.opacity/100))},children:[s?p.jsx("div",{className:W?"top1-halo pulse":"top1-halo",style:{position:"absolute",inset:"2px 2px 2px 2px",borderRadius:16,pointerEvents:"none",background:"radial-gradient(circle at 12% 50%, rgba(255,80,120,.35), transparent 24%), radial-gradient(circle at 72% 50%, rgba(255,195,0,.18), transparent 40%)",filter:"blur(8px)",opacity:.95}}):null,o.alignRight?p.jsxs(p.Fragment,{children:[k,j,A]}):p.jsxs(p.Fragment,{children:[A,j,k]})]},`${i.position}-${i.name}-${i.value}`)})})})}),p.jsx("style",{children:`
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
