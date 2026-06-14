import{i as e,n as t,t as n}from"./jsx-runtime-CY_jTiqj.js";import{u as r}from"./index-DxQ8bueZ.js";import{a as i}from"./overlay-url-DoP8AAQQ.js";import{a,i as o,s}from"./top-record-widget-qBPzOyy2.js";var c=e(t()),l=n(),u=`http://127.0.0.1:35942`,d={topLike:{id:`topLike`,metricIcon:`heart`,configKey:`liveplay.toplike.config`,snapshotKey:`liveplay.toplike.snapshot`,channel:`liveplay-toplike-channel`,snapshotType:`top-like-sync`,rankKey:`likes`,queryBoard:`topLike`},topGifts:{id:`topGifts`,metricIcon:`coin`,configKey:`liveplay.topgifts.config`,snapshotKey:`liveplay.topgifts.snapshot`,channel:`liveplay-topgifts-channel`,snapshotType:`top-gifts-sync`,rankKey:`gifts`,queryBoard:`topGifts`},weeklyRank:{id:`weeklyRank`,metricIcon:`coin`,configKey:`liveplay.weeklyrank.config`,snapshotKey:`liveplay.weeklyrank.snapshot`,channel:`liveplay-weeklyrank-channel`,snapshotType:`weekly-rank-sync`,rankKey:`weekly`,queryBoard:`weeklyRank`},monthlyRank:{id:`monthlyRank`,metricIcon:`coin`,configKey:`liveplay.monthlyrank.config`,snapshotKey:`liveplay.monthlyrank.snapshot`,channel:`liveplay-monthlyrank-channel`,snapshotType:`monthly-rank-sync`,rankKey:`monthly`,queryBoard:`monthlyRank`},topGift:{id:`topGift`,metricIcon:`coin`,configKey:`liveplay.topgift.config`,snapshotKey:`liveplay.topgift.snapshot`,channel:`liveplay-topgift-channel`,snapshotType:`top-gift-sync`,rankKey:`topGift`,queryBoard:`topGift`},topCombo:{id:`topCombo`,metricIcon:`coin`,configKey:`liveplay.topcombo.config`,snapshotKey:`liveplay.topcombo.snapshot`,channel:`liveplay-topcombo-channel`,snapshotType:`top-combo-sync`,rankKey:`topCombo`,queryBoard:`topCombo`}};function f(e){return e.trim().replace(/\/$/,``)}function p(e,t){return`${f(e)}${t.startsWith(`/`)?t:`/${t}`}`}var m={theme:`dark`,opacity:100,topCount:10,showAvatars:!0,showMedals:!0,showCrown:!0,showMetricIcon:!0,displaySeconds:8,width:820,compactMode:!1,nameFont:`display`,valueFont:`display`,nameColor:`#fff200`,valueColor:`#22d3ee`,nameFontSize:100,valueFontSize:100,nameLetterSpacing:0,valueLetterSpacing:0,lineSpacing:100,alignRight:!1},h={display:`"Trebuchet MS", "Arial Black", Impact, sans-serif`,default:`Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif`,tech:`"Lucida Console", "Courier New", monospace`,compact:`"Arial Narrow", "Roboto Condensed", "Segoe UI", sans-serif`,serif:`Georgia, "Times New Roman", serif`};function g(e){return h[e]||h.default}function _(e){let t=typeof e==`string`?JSON.parse(e):e??{};return{...m,...t,topCount:t.topCount===3||t.topCount===5||t.topCount===10?t.topCount:m.topCount,theme:t.theme===`light`?`light`:`dark`,opacity:typeof t.opacity==`number`?Math.max(0,Math.min(100,t.opacity)):m.opacity,displaySeconds:typeof t.displaySeconds==`number`?Math.max(1,Math.min(60,t.displaySeconds)):m.displaySeconds,width:typeof t.width==`number`?Math.max(420,Math.min(1400,t.width)):m.width,nameFont:t.nameFont==="default"||t.nameFont===`display`||t.nameFont===`tech`||t.nameFont===`compact`||t.nameFont===`serif`?t.nameFont:m.nameFont,valueFont:t.valueFont==="default"||t.valueFont===`display`||t.valueFont===`tech`||t.valueFont===`compact`||t.valueFont===`serif`?t.valueFont:m.valueFont,nameColor:typeof t.nameColor==`string`&&t.nameColor.trim()?t.nameColor:m.nameColor,valueColor:typeof t.valueColor==`string`&&t.valueColor.trim()?t.valueColor:m.valueColor,nameFontSize:typeof t.nameFontSize==`number`?Math.max(70,Math.min(180,t.nameFontSize)):m.nameFontSize,valueFontSize:typeof t.valueFontSize==`number`?Math.max(70,Math.min(180,t.valueFontSize)):m.valueFontSize,nameLetterSpacing:typeof t.nameLetterSpacing==`number`?Math.max(-2,Math.min(12,t.nameLetterSpacing)):m.nameLetterSpacing,valueLetterSpacing:typeof t.valueLetterSpacing==`number`?Math.max(-2,Math.min(12,t.valueLetterSpacing)):m.valueLetterSpacing,lineSpacing:typeof t.lineSpacing==`number`?Math.max(70,Math.min(180,t.lineSpacing)):m.lineSpacing,alignRight:typeof t.alignRight==`boolean`?t.alignRight:m.alignRight}}function v(e){try{let t=window.localStorage.getItem(e.configKey);return t?_(t):m}catch{return m}}function y(e,t){let n=typeof t==`string`?JSON.parse(t):t??{};return{...a(e.id),...n}}function b(e){try{let t=window.localStorage.getItem(e.configKey);return t?y(e,t):a(e.id)}catch{return a(e.id)}}function x(e){try{let t=window.localStorage.getItem(e.snapshotKey);if(!t)return null;let n=JSON.parse(t);return n.type===e.snapshotType?n:null}catch{return null}}function S(e){return e.id===`topLike`?`likes`:e.id===`topGifts`?`gifts`:e.id===`weeklyRank`?`weeklyCoins`:e.id===`monthlyRank`?`monthlyCoins`:e.id===`topGift`?`topGift`:`topCombo`}function C(e){return e===`gold`?`🥇`:e===`silver`?`🥈`:e===`bronze`?`🥉`:null}function ee(e){return new Intl.NumberFormat(`pt-BR`).format(e)}function w(e){let t=[e?.avatar,e?.avatarUrl,e?.profilePictureUrl,e?.profilePicture,e?.photoUrl,e?.pictureUrl,e?.imageUrl,e?.user?.avatar,e?.user?.avatarUrl,e?.user?.profilePictureUrl,e?.user?.profilePicture,e?.user?.photoUrl];for(let e of t){let t=String(e||``).trim();if(t)return t}}function T(e,t,n){let r=t===`weekly`?`weeklyCoins`:t===`monthly`?`monthlyCoins`:t;return(Array.isArray(e?.ranks?.[r]?.entries)?e.ranks[r].entries:[]).slice(0,n).map((e,t)=>({position:t+1,name:String(e?.nickname||e?.username||e?.name||e?.user?.nickname||e?.user?.username||`Usuário ${t+1}`),value:Math.max(0,Number(e?.value||0)),avatar:w(e),medal:t===0?`gold`:t===1?`silver`:t===2?`bronze`:void 0,crowned:t===0,giftId:String(e?.giftId||``).trim()||void 0,giftName:String(e?.giftName||``).trim()||void 0,giftImageUrl:String(e?.giftImageUrl||``).trim()||void 0,comboCount:Math.max(0,Number(e?.comboCount||0))||void 0,coinValue:Math.max(0,Number(e?.coinValue||0))||void 0}))}function te(e,t){return e.map(e=>`${e.position}:${e.name}:${e.value}`).join(`|`)+`::${t||0}`}function ne(e,t,n,r,i=100,a=`display`){return e===`coin`?(0,l.jsx)(`span`,{className:n?`metric-icon metric-coin event-coin`:`metric-icon metric-coin`,style:{color:r,fontSize:i/100*(t?17:15),lineHeight:1,fontFamily:g(a),filter:t?`drop-shadow(0 0 8px rgba(255,207,63,.45))`:`none`},children:`●`}):(0,l.jsx)(`span`,{className:n?`metric-icon metric-heart event-heart`:`metric-icon metric-heart`,style:{color:`#ff2c55`,fontSize:t?17:15,lineHeight:1,filter:t?`drop-shadow(0 0 8px rgba(255,44,85,.45))`:`none`},children:`♥`})}function E(e,t){let n=Math.max(0,Math.min(1,Number(e)/100));return Math.max(0,Math.min(t,n*t))}function re(e,t){let n=E(t,e===`light`?.98:.88),r=E(t,e===`light`?.96:.1),i=E(t,e===`light`?.72:.035),a=E(t,.08),o=E(t,e===`light`?.16:.14),s=E(t,e===`light`?.18:.28);return e===`light`?{shellBackground:`rgba(248,250,252,${n})`,shellBorder:`1px solid rgba(15,23,42,${o})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${r}), rgba(226,232,240,${i}))`,rowBackground:`rgba(255,255,255,${i})`,avatarBackground:`rgba(15,23,42,${a})`,avatarInitialColor:`#0f172a`,defaultNameColor:`#0f172a`,defaultValueColor:`#0369a1`,topHalo:`radial-gradient(circle at 12% 50%, rgba(59,130,246,.22), transparent 24%), radial-gradient(circle at 72% 50%, rgba(250,204,21,.22), transparent 42%)`,shadow:s>0?`0 18px 36px rgba(15,23,42,${s})`:`none`}:{shellBackground:`rgba(4,7,18,${n})`,shellBorder:`1px solid rgba(255,255,255,${o})`,rowBackgroundTop:`linear-gradient(90deg, rgba(255,255,255,${r}), rgba(255,255,255,${i}))`,rowBackground:`rgba(255,255,255,${i})`,avatarBackground:`rgba(255,255,255,${a})`,avatarInitialColor:`#d1d5db`,defaultNameColor:`#fff200`,defaultValueColor:`#22d3ee`,topHalo:`radial-gradient(circle at 12% 50%, rgba(255,80,120,.35), transparent 24%), radial-gradient(circle at 72% 50%, rgba(255,195,0,.18), transparent 42%)`,shadow:s>0?`0 18px 40px rgba(0,0,0,${s})`:`none`}}function D(){let e=(0,c.useMemo)(()=>new URLSearchParams(window.location.search),[]),t=e.get(`board`)||`topLike`,n=e.get(`sourceId`)||t,f=t===`topGifts`?d.topGifts:t===`weeklyRank`?d.weeklyRank:t===`monthlyRank`?d.monthlyRank:t===`topGift`?d.topGift:t===`topCombo`?d.topCombo:d.topLike,h=e.get(`bridgeUrl`),w=typeof window<`u`&&window.liveplay!==void 0,[E,D]=(0,c.useState)(()=>h||i(e,`/overlay-bridge`,u)),[O,k]=(0,c.useState)(()=>f.id===`topGift`||f.id===`topCombo`?b(f):v(f)),[A,j]=(0,c.useState)(()=>x(f)),[ie,M]=(0,c.useState)([]),[ae,N]=(0,c.useState)(!1),[P,F]=(0,c.useState)(!1),[I,L]=(0,c.useState)(()=>new Set),R=(0,c.useRef)(A?.updatedAt??0),z=(0,c.useRef)(``),B=(0,c.useRef)(Number(O?.updatedAt||0)),V=(0,c.useRef)(!!O?.updatedAt),H=e=>{if(!e){V.current||k(f.id===`topGift`||f.id===`topCombo`?a(f.id):m);return}let t=f.id===`topGift`||f.id===`topCombo`?y(f,e):_(e),n=Number(t?.updatedAt||0);if(n>0){if(n<B.current)return;B.current=n,V.current=!0}else if(V.current)return;k(t)},U=e=>e?.payload&&typeof e.payload==`object`?e.payload:e,W=(e,t)=>{let r=String(e?.board||``).trim(),i=String(e?.sourceId||``).trim(),a=new Set([f.id,f.queryBoard,S(f),t||``,n].filter(Boolean));return!(r&&!a.has(r)||i&&!a.has(i))},G=e=>{let t=U(e);if(!t||t.type!==`rank-config-sync`||!W(t))return;let n=Number(t.updatedAt||t.config?.updatedAt||0);n&&n<B.current||H({...t.config,updatedAt:n||Number(t.config?.updatedAt||Date.now())})};(0,c.useEffect)(()=>{let e=document.documentElement,t=document.body,n=document.getElementById(`root`),r=e.style.background,i=t.style.background,a=n?.style.background??``;return e.style.background=`transparent`,t.style.background=`transparent`,n&&(n.style.background=`transparent`),()=>{e.style.background=r,t.style.background=i,n&&(n.style.background=a)}},[]),(0,c.useEffect)(()=>{w&&window.liveplay?.app?.getOverlayBridgeUrl?.().then(e=>{e?.url&&D(e.url)}).catch(()=>void 0)},[w]),(0,c.useEffect)(()=>{H(f.id===`topGift`||f.id===`topCombo`?b(f):v(f));let e=x(f);j(e),R.current=e?.updatedAt??0},[f]),(0,c.useEffect)(()=>{let e=!0;return(async()=>{let[t,n]=await Promise.all([r(f.configKey,null),r(f.snapshotKey,null)]);e&&(t&&H(t),n&&n.type===f.snapshotType&&(!n.board||n.board===f.id)&&(j(n),R.current=n.updatedAt??0))})().catch(()=>void 0),()=>{e=!1}},[f]),(0,c.useEffect)(()=>{let e=()=>H(f.id===`topGift`||f.id===`topCombo`?b(f):v(f)),t=e=>{let t=U(e);if(!t||t.type!==f.snapshotType||!W(t))return;e=t;let n=Number(e.updatedAt||e.at||Date.now());n<R.current||(R.current=n,j({...e,updatedAt:n}))},r=n=>{n.key===f.configKey&&e(),n.key===f.snapshotKey&&t(x(f))},i=e=>{t(e.detail)},a=e=>{G(e.detail)};window.addEventListener(`storage`,r),window.addEventListener(`liveplay:${f.id}-snapshot`,i),window.addEventListener(`liveplay:${f.id}-config`,a);let o=null;try{o=new BroadcastChannel(f.channel),o.onmessage=e=>{let n=U(e.data);if(n?.type===`rank-config-sync`){G(n);return}t(n)}}catch{}let s=window.setInterval(()=>{document.visibilityState===`visible`&&H(f.id===`topGift`||f.id===`topCombo`?b(f):v(f))},2500),c=!1,l=S(f),u=e=>{let t=U(e);if(!t||t?.type!==`rank-overlay-sync`||!W(t,l))return;let n=t?.ranks;n&&(N(!!t?.liveActive),M(T({ranks:n},f.rankKey,10)))},d=async()=>{try{let[e,r,i]=await Promise.all([fetch(p(E,`/snapshot?type=rank-config-sync&board=${f.id}&sourceId=${encodeURIComponent(n)}`),{cache:`no-store`}).then(e=>e.json()).catch(()=>null),fetch(p(E,`/snapshot?type=${f.snapshotType}&board=${f.id}&sourceId=${encodeURIComponent(n)}`),{cache:`no-store`}).then(e=>e.json()).catch(()=>null),fetch(p(E,`/snapshot?type=rank-overlay-sync&board=${l}&sourceId=${encodeURIComponent(n)}`),{cache:`no-store`}).then(e=>e.json()).catch(()=>null)]);if(c)return;let a=U(e?.payload??e);a?.type===`rank-config-sync`&&G(a);let o=U(r?.payload??r);o?.type===f.snapshotType&&t({...o,updatedAt:Number(o.updatedAt||o.at||Date.now())});let s=U(i?.payload??i);s?.type===`rank-overlay-sync`&&u(s)}catch{}},m=[],h=e=>{try{let n=new EventSource(p(E,e));n.onmessage=e=>{try{let n=U(JSON.parse(e.data));if(n?.type===`rank-config-sync`){G(n);return}if(n?.type===f.snapshotType){t({...n,updatedAt:Number(n.updatedAt||n.at||Date.now())});return}n?.type===`rank-overlay-sync`&&u(n)}catch{}},m.push(n)}catch{}};h(`/events?type=rank-config-sync&board=${encodeURIComponent(f.id)}&sourceId=${encodeURIComponent(n)}`),h(`/events?type=${encodeURIComponent(f.snapshotType)}&board=${encodeURIComponent(f.id)}&sourceId=${encodeURIComponent(n)}`),h(`/events?type=rank-overlay-sync&board=${encodeURIComponent(l)}&sourceId=${encodeURIComponent(n)}`),h(`/events`);let g=window.liveplay?.app?.onOverlaySync?.(e=>{let n=U(e);if(n?.type===`rank-config-sync`){G(n);return}if(n?.type===f.snapshotType){t({...n,updatedAt:Number(n.updatedAt||n.at||Date.now())});return}n?.type===`rank-overlay-sync`&&u(n)}),_=async()=>{try{let e=window.liveplay?.app;if(!e?.getState||!e?.getStatus){await d();return}let[t,n]=await Promise.all([e.getState(),e.getStatus()]);if(c)return;N(!!n?.tiktokConnected),M(T(t,f.rankKey,10)),await d()}catch{c||await d()}};_();let y=[80,220,500,900,1500,2400].map(e=>window.setTimeout(()=>{c||d()},e)),C=window.setInterval(()=>{document.visibilityState===`visible`&&_()},5e3);return()=>{c=!0,y.forEach(e=>window.clearTimeout(e)),window.removeEventListener(`storage`,r),window.removeEventListener(`liveplay:${f.id}-snapshot`,i),window.removeEventListener(`liveplay:${f.id}-config`,a),window.clearInterval(s),window.clearInterval(C),o?.close(),m.forEach(e=>e.close()),typeof g==`function`&&g()}},[E,w,f,n]);let K=f.id===`topGift`||f.id===`topCombo`?1:O.topCount,oe=f.id===`weeklyRank`||f.id===`monthlyRank`,q=!!A?.visible&&A.mode===`test`&&A.entries?.length,J=q?A.entries.slice(0,K):ae||oe?ie.slice(0,K):[];if((0,c.useEffect)(()=>{if(!q||!A)return;let e=f.id===`topGift`||f.id===`topCombo`?8:Math.max(1,Math.min(60,Number(O.displaySeconds||m.displaySeconds))),t=window.setTimeout(()=>{j(e=>!e||e.mode!==`test`||e.updatedAt!==A.updatedAt?e:{...e,visible:!1,updatedAt:Date.now()})},e*1e3+450);return()=>window.clearTimeout(t)},[q,A?.updatedAt,f.id,O]),(0,c.useEffect)(()=>{let e=te(J,q?A?.updatedAt:0);if(!e){z.current=``;return}if(z.current&&z.current!==e){F(!0);let t=window.setTimeout(()=>F(!1),950);return z.current=e,()=>window.clearTimeout(t)}z.current=e},[J,q,A?.updatedAt]),(f.id===`topGift`||f.id===`topCombo`)&&J.length){let e=y(f,O),t=J[0],n=s({username:t.name,nickname:t.name,avatarUrl:t.avatar,giftId:t.giftId,giftName:t.giftName,giftImageUrl:t.giftImageUrl,comboCount:t.comboCount,coinValue:t.coinValue,value:t.value},f.id);return n?(0,l.jsx)(`div`,{style:{width:`100%`,height:`100%`,background:`transparent`,display:`flex`,alignItems:`flex-start`,justifyContent:`flex-start`,padding:0,overflow:`hidden`},children:(0,l.jsx)(`div`,{style:{width:390,maxWidth:`100%`,padding:10,boxSizing:`border-box`,background:`transparent`,border:`none`,borderRadius:16,boxShadow:`none`},children:(0,l.jsx)(o,{kind:f.id,config:e,entry:n,embedded:!0})})}):(0,l.jsx)(`div`,{style:{width:`100%`,height:`100%`,background:`transparent`}})}if(!J.length)return(0,l.jsx)(`div`,{style:{width:`100%`,height:`100%`,background:`transparent`}});let Y=O,se=Y.compactMode?8:10,X=Y.compactMode?42:48,Z=Y.compactMode?30:34,Q=(Y.compactMode?13:15)*(Y.nameFontSize/100),ce=(Y.compactMode?12:14)*(Y.valueFontSize/100),le=2*(Y.lineSpacing/100),$=re(Y.theme,Y.opacity);return(0,l.jsxs)(l.Fragment,{children:[(0,l.jsx)(`div`,{style:{width:`100%`,height:`100%`,background:`transparent`,display:`flex`,alignItems:`flex-start`,justifyContent:Y.alignRight?`flex-end`:`flex-start`,padding:0,overflow:`hidden`},children:(0,l.jsx)(`div`,{style:{width:Math.min(Y.width,1400),minHeight:180,borderRadius:16,border:$.shellBorder,background:$.shellBackground,padding:Y.compactMode?`8px 10px`:`10px 12px`,boxSizing:`border-box`,overflow:`hidden`,boxShadow:$.shadow,isolation:`isolate`},children:(0,l.jsx)(`div`,{style:{display:`grid`,gap:se,maxWidth:`100%`},children:J.map((e,t)=>{let n=C(e.medal),r=t===0,i=t===1,a=t===2,o=P&&(r||i||a),s=`${e.position}:${e.name}:${e.avatar||``}`,c=!!(e.avatar&&!I.has(s)),u=(0,l.jsxs)(`div`,{style:{minWidth:0,display:`grid`,gap:le,position:`relative`,zIndex:2,justifyItems:Y.alignRight?`end`:`start`},children:[(0,l.jsx)(`div`,{style:{color:Y.theme===`light`&&Y.nameColor===m.nameColor?$.defaultNameColor:Y.nameColor,fontFamily:g(Y.nameFont),fontWeight:r?900:800,fontSize:r?Q+2:Q,lineHeight:1.1,letterSpacing:`${Y.nameLetterSpacing}px`,whiteSpace:`nowrap`,overflow:`hidden`,textOverflow:`ellipsis`,textShadow:r?`0 0 10px rgba(255,242,0,.28)`:`none`},children:e.name}),(0,l.jsxs)(`div`,{style:{display:`flex`,alignItems:`center`,gap:6,justifyContent:Y.alignRight?`flex-end`:`flex-start`,color:Y.theme===`light`&&Y.valueColor===m.valueColor?$.defaultValueColor:Y.valueColor,fontFamily:g(Y.valueFont),fontWeight:800,fontSize:ce,letterSpacing:`${Y.valueLetterSpacing}px`},children:[Y.showMetricIcon?ne(f.metricIcon,r,o,Y.theme===`light`&&Y.valueColor===m.valueColor?$.defaultValueColor:Y.valueColor,Y.valueFontSize,Y.valueFont):null,(0,l.jsx)(`span`,{className:o?`value-bump`:``,style:{display:`inline-block`,textShadow:r?`0 0 10px rgba(34,211,238,.22)`:`none`},children:ee(e.value)})]})]}),d=Y.showAvatars?(0,l.jsxs)(`div`,{style:{position:`relative`,width:X,height:X,minWidth:X,minHeight:X,maxWidth:X,maxHeight:X,borderRadius:`50%`,overflow:`visible`,border:r?`2px solid rgba(255,215,0,.95)`:`2px solid rgba(255,193,7,.75)`,background:$.avatarBackground,display:`grid`,placeItems:`center`,zIndex:2,boxShadow:r?`0 0 18px rgba(255,210,0,.45)`:i||a?`0 0 12px rgba(255,255,255,.12)`:`none`},children:[c?(0,l.jsx)(`img`,{src:e.avatar,alt:``,draggable:!1,style:{position:`absolute`,inset:0,width:X,height:X,minWidth:X,minHeight:X,maxWidth:X,maxHeight:X,objectFit:`cover`,objectPosition:`center`,borderRadius:`50%`,display:`block`,lineHeight:0},referrerPolicy:`no-referrer`,onError:()=>{L(e=>{let t=new Set(e);return t.add(s),t})}}):(0,l.jsx)(`span`,{style:{color:$.avatarInitialColor,fontSize:Math.max(18,X*.42),fontWeight:800},children:e.name.charAt(0).toUpperCase()}),Y.showCrown&&e.crowned?(0,l.jsx)(`span`,{className:P?`crown crown-bounce`:`crown`,style:{position:`absolute`,top:-18,left:8,fontSize:Y.compactMode?22:24,zIndex:3,filter:`drop-shadow(0 0 10px rgba(255,200,0,.55))`},children:`👑`}):null]}):null,p=Y.showMedals?(0,l.jsx)(`div`,{style:{color:r?`#ffb72e`:e.position<=3?`#ffffff`:`#ff3434`,fontSize:r?24:e.position<=3?20:32,fontWeight:900,lineHeight:1,textAlign:`center`,position:`relative`,zIndex:2,textShadow:r?`0 0 12px rgba(255,183,46,.65)`:`none`},children:n||(0,l.jsxs)(`span`,{style:{display:`inline-block`,minWidth:20},children:[e.position,`.`]})}):null,h=Y.alignRight?`minmax(0, 1fr)${Y.showAvatars?` ${X}px`:``}${Y.showMedals?` ${Z}px`:``}`:`${Y.showMedals?`${Z}px `:``}${Y.showAvatars?`${X}px `:``}minmax(0, 1fr)`;return(0,l.jsxs)(`div`,{className:[`lp-rank-row`,r?`top1`:``,i?`top2`:``,a?`top3`:``,o?`event-pulse`:``].join(` `).trim(),style:{display:`grid`,gridTemplateColumns:h,alignItems:`center`,columnGap:10,minHeight:Y.compactMode?46:54,padding:r?`6px 8px`:`4px 6px`,borderRadius:14,background:r?$.rowBackgroundTop:$.rowBackground,border:Y.theme===`light`?`1px solid rgba(15,23,42,.06)`:`1px solid rgba(255,255,255,.045)`,position:`relative`,transform:o&&r?`scale(1.035)`:`scale(1)`,transition:`transform 220ms ease, filter 220ms ease`},children:[r?(0,l.jsx)(`div`,{className:P?`top1-halo pulse`:`top1-halo`,style:{position:`absolute`,inset:`2px 2px 2px 2px`,borderRadius:16,pointerEvents:`none`,background:$.topHalo,filter:`blur(8px)`,opacity:.95}}):null,Y.alignRight?(0,l.jsxs)(l.Fragment,{children:[u,d,p]}):(0,l.jsxs)(l.Fragment,{children:[p,d,u]})]},`${e.position}-${e.name}-${e.value}`)})})})}),(0,l.jsx)(`style`,{children:`
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
      `})]})}export{D as RanksOverlayPage};