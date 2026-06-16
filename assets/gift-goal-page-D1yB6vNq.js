import{i as e,n as t,t as n}from"./jsx-runtime-CY_jTiqj.js";import{t as r}from"./card-DUJYNqT2.js";import{p as i,u as a}from"./index-BogyQEMs.js";import{t as o}from"./brazil-gifts-CCGVaM9q.js";import{a as s,n as c,r as l,t as u}from"./overlay-url-DoP8AAQQ.js";import{a as d,i as f,n as p,o as m,r as h,t as g}from"./gift-goal-KXUtgoQ3.js";var _=e(t()),v=n(),y=`http://127.0.0.1:35942`,b=`https://liveplay-backend.onrender.com/overlay-bridge/emit`;function x(e){let t=String(e||``).trim().replace(/\/$/,``);return t?t.endsWith(`/emit`)?t:`${t}/emit`:``}function S(e){let t=new Set;return[e,y,b].forEach(e=>{let n=x(e);n&&t.add(n)}),Array.from(t)}function C(e,t){if(e.config){try{window.localStorage.setItem(g,JSON.stringify(e.config))}catch{}i(g,e.config).catch(()=>void 0)}try{let t=new BroadcastChannel(p);t.postMessage(e),t.close()}catch{}typeof window.liveplay?.app?.emitOverlaySync==`function`&&window.liveplay.app.emitOverlaySync(e).catch(()=>void 0);for(let n of S(t))try{fetch(n,{method:`POST`,headers:{"Content-Type":`application/json`},body:JSON.stringify(e)}).catch(()=>void 0)}catch{}}function w(e){return new Intl.NumberFormat(`pt-BR`).format(Math.max(0,Number(e||0)))}function T(e,t,n,r){let i=Number(e);return Number.isFinite(i)?Math.max(n,Math.min(r,i)):t}function E({value:e,min:t,max:n,step:r=1,onCommit:i}){let[a,o]=(0,_.useState)(String(e));return(0,_.useEffect)(()=>{o(String(e))},[e]),(0,v.jsx)(`input`,{type:`number`,min:t,max:n,step:r,value:a,onChange:e=>o(e.target.value),onBlur:(0,_.useCallback)(()=>{let r=T(a,e,t,n);o(String(r)),r!==e&&i(r)},[a,n,t,i,e]),onKeyDown:e=>{e.key===`Enter`&&e.currentTarget.blur()}})}function D(){let[e,t]=(0,_.useState)(()=>f()),[n,p]=(0,_.useState)(``),b=s((0,_.useMemo)(()=>new URLSearchParams(window.location.search),[]),`/overlay-bridge`,y),x=u(`/overlay-bridge`,`giftGoal`),S=c(l(),`/overlay-bridge`,`giftGoal`);(0,_.useEffect)(()=>{let e=!0;return(async()=>{let n=await a(g,null).catch(()=>null);e&&n&&t(m(n));try{let n=window.localStorage.getItem(g);e&&n&&t(m(JSON.parse(n)))}catch{}})(),()=>{e=!1}},[]);let T=(0,_.useCallback)((e,n=`manual`)=>{let r=m(e);t(r),C({type:`gift-goal-config`,at:Date.now(),config:r,reason:n},b)},[b]);(0,_.useEffect)(()=>{let e=window.liveplay?.app?.onOverlaySync?.(e=>{e?.type===`gift-goal-event`&&e.event&&t(t=>{let n=h(t,e.event);return i(g,n).catch(()=>void 0),n}),e?.type===`gift-goal-reset`&&t(e=>{let t=m({...e,goals:e.goals.map(e=>({...e,current:0}))});return i(g,t).catch(()=>void 0),t})});return()=>{typeof e==`function`&&e()}},[]);let D=(0,_.useMemo)(()=>{let e=n.trim().toLowerCase();return(e?o.filter(t=>`${t.name} ${t.id} ${t.coin}`.toLowerCase().includes(e)):o).slice(0,80)},[n]),O=t=>{if(e.goals.length>=5)return;let n=d();T({...e,goals:[...e.goals,{...n,giftId:t.id,giftName:t.name,giftImageUrl:t.imageUrl||``,accent:t.accent||n.accent}]},`add-goal`)},k=(t,n)=>{T({...e,goals:e.goals.map(e=>e.id===t?{...e,...n}:e)},`update-goal`)},A=t=>{T({...e,goals:e.goals.filter(e=>e.id!==t)},`remove-goal`)},j=()=>{T({...e,goals:e.goals.map(e=>({...e,current:0}))},`reset-counts`),C({type:`gift-goal-reset`,at:Date.now(),reason:`manual-reset`},b)},M=()=>{let t=f();T({...e,position:t.position,scale:t.scale,titleSize:t.titleSize,giftSize:t.giftSize,countSize:t.countSize,opacity:t.opacity,skin:t.skin,showHeader:t.showHeader,effect:t.effect,titleColor:t.titleColor,countColor:t.countColor,targetColor:t.targetColor,transition:t.transition,rotateSeconds:t.rotateSeconds,skipCompleted:t.skipCompleted,orderMode:t.orderMode},`restore-visual-defaults`)},N=t=>{T(h(e,{id:`gift-goal-test-${Date.now()}`,giftId:t.giftId,giftName:t.giftName,giftImageUrl:t.giftImageUrl,repeatCount:1,isTest:!0}),`test-event`)};return(0,v.jsxs)(`div`,{className:`page-stack`,children:[(0,v.jsx)(`style`,{children:`
        .gift-goal-row {
          display: grid !important;
          grid-template-columns: 64px minmax(260px, 1fr) 120px auto !important;
          align-items: center !important;
          gap: 12px !important;
        }
        .gift-goal-actions {
          display: flex;
          align-items: center;
          justify-content: flex-end;
          gap: 10px;
          flex-wrap: nowrap;
          white-space: nowrap;
        }
        .gift-goal-actions button {
          min-width: max-content;
        }
        .gift-goal-main .gift-goal-inline-count {
          display: inline-flex;
          align-items: baseline;
          gap: 4px;
          font-weight: 900;
          color: #ffffff;
        }
        .gift-goal-main .gift-goal-inline-count strong {
          font-size: 18px;
          line-height: 1;
          color: #ffffff;
        }
        .gift-goal-main .gift-goal-inline-count span {
          font-size: 14px;
          color: #ffd42a;
        }
        .gift-goal-main .gift-goal-status {
          font-size: 12px;
          color: #aeb9d6;
        }
        .gift-goal-enable-row {
          display: flex;
          align-items: center;
          gap: 10px;
          flex-wrap: wrap;
          margin-top: -4px;
        }
        @media (max-width: 980px) {
          .gift-goal-row {
            grid-template-columns: 56px 1fr !important;
          }
          .gift-goal-row .small-field,
          .gift-goal-actions {
            grid-column: 1 / -1;
          }
          .gift-goal-actions {
            justify-content: flex-start;
            flex-wrap: wrap;
          }
        }
      `}),(0,v.jsxs)(r,{title:`Meta de Gifts`,subtitle:`Escolha até 5 presentes específicos e mostre o progresso no overlay.`,children:[(0,v.jsxs)(`div`,{className:`settings-grid`,children:[(0,v.jsxs)(`label`,{className:`field`,children:[(0,v.jsx)(`span`,{children:`Título do overlay`}),(0,v.jsx)(`input`,{value:e.title,onChange:t=>T({...e,title:t.target.value})})]}),(0,v.jsxs)(`label`,{className:`field`,children:[(0,v.jsx)(`span`,{children:`Status da meta`}),(0,v.jsxs)(`select`,{value:e.enabled?`enabled`:`disabled`,onChange:t=>T({...e,enabled:t.target.value===`enabled`}),children:[(0,v.jsx)(`option`,{value:`enabled`,children:`Ativada`}),(0,v.jsx)(`option`,{value:`disabled`,children:`Desativada`})]})]}),(0,v.jsx)(`div`,{className:`gift-goal-enable-row`,children:(0,v.jsx)(`span`,{className:`muted-text`,children:`Desliga ou liga o overlay inteiro sem apagar as metas configuradas.`})}),(0,v.jsxs)(`label`,{className:`field`,children:[(0,v.jsx)(`span`,{children:`Posição`}),(0,v.jsxs)(`select`,{value:e.position,onChange:t=>T({...e,position:t.target.value}),children:[(0,v.jsx)(`option`,{value:`center`,children:`Centro`}),(0,v.jsx)(`option`,{value:`top-left`,children:`Topo esquerdo`}),(0,v.jsx)(`option`,{value:`top-right`,children:`Topo direito`}),(0,v.jsx)(`option`,{value:`bottom-left`,children:`Baixo esquerdo`}),(0,v.jsx)(`option`,{value:`bottom-right`,children:`Baixo direito`})]})]}),(0,v.jsxs)(`label`,{className:`field`,children:[(0,v.jsx)(`span`,{children:`Transição do loop`}),(0,v.jsxs)(`select`,{value:e.transition||`fade`,onChange:t=>T({...e,transition:t.target.value}),children:[(0,v.jsx)(`option`,{value:`loop`,children:`Loop pulsante`}),(0,v.jsx)(`option`,{value:`fade`,children:`Fade`}),(0,v.jsx)(`option`,{value:`slide`,children:`Slide`}),(0,v.jsx)(`option`,{value:`flip`,children:`Flip 3D`})]})]}),(0,v.jsxs)(`label`,{className:`field`,children:[(0,v.jsx)(`span`,{children:`Tempo de cada gift (s)`}),(0,v.jsx)(E,{value:e.rotateSeconds||5,min:2,max:60,step:1,onCommit:t=>T({...e,rotateSeconds:t})})]}),(0,v.jsxs)(`label`,{className:`field`,children:[(0,v.jsx)(`span`,{children:`Ordem do loop`}),(0,v.jsxs)(`select`,{value:e.orderMode||`closest`,onChange:t=>T({...e,orderMode:t.target.value}),children:[(0,v.jsx)(`option`,{value:`closest`,children:`Mais perto da meta primeiro`}),(0,v.jsx)(`option`,{value:`rotation`,children:`Rotação normal`})]})]}),(0,v.jsxs)(`label`,{className:`field`,children:[(0,v.jsx)(`span`,{children:`Efeito extra`}),(0,v.jsxs)(`select`,{value:e.effect||`none`,onChange:t=>T({...e,effect:t.target.value}),children:[(0,v.jsx)(`option`,{value:`none`,children:`Sem efeito extra`}),(0,v.jsx)(`option`,{value:`neon`,children:`Neon LivePlay`}),(0,v.jsx)(`option`,{value:`pulse`,children:`Pulsando`}),(0,v.jsx)(`option`,{value:`float`,children:`Flutuando`}),(0,v.jsx)(`option`,{value:`pop`,children:`Pop no gift`})]})]}),(0,v.jsxs)(`label`,{className:`field`,children:[(0,v.jsx)(`span`,{children:`Estilo`}),(0,v.jsxs)(`select`,{value:e.skin,onChange:t=>T({...e,skin:t.target.value}),children:[(0,v.jsx)(`option`,{value:`neon`,children:`Sem fundo / neon`}),(0,v.jsx)(`option`,{value:`clean`,children:`Clean`}),(0,v.jsx)(`option`,{value:`glass`,children:`Glass discreto`})]})]}),(0,v.jsxs)(`label`,{className:`field`,children:[(0,v.jsx)(`span`,{children:`Tamanho do overlay`}),(0,v.jsx)(E,{value:e.scale||1,min:.4,max:2.4,step:.05,onCommit:t=>T({...e,scale:t})})]}),(0,v.jsxs)(`label`,{className:`field`,children:[(0,v.jsx)(`span`,{children:`Tamanho do título`}),(0,v.jsx)(E,{value:e.titleSize||36,min:16,max:96,step:1,onCommit:t=>T({...e,titleSize:t})})]}),(0,v.jsxs)(`label`,{className:`field`,children:[(0,v.jsx)(`span`,{children:`Tamanho do gift`}),(0,v.jsx)(E,{value:e.giftSize||180,min:64,max:420,step:1,onCommit:t=>T({...e,giftSize:t})})]}),(0,v.jsxs)(`label`,{className:`field`,children:[(0,v.jsx)(`span`,{children:`Tamanho dos números`}),(0,v.jsx)(E,{value:e.countSize||48,min:20,max:120,step:1,onCommit:t=>T({...e,countSize:t})})]})]}),(0,v.jsxs)(`div`,{className:`settings-grid gift-visual-grid`,children:[(0,v.jsxs)(`label`,{className:`field color-field`,children:[(0,v.jsx)(`span`,{children:`Cor do título`}),(0,v.jsx)(`input`,{type:`color`,value:e.titleColor||`#ffffff`,onChange:t=>T({...e,titleColor:t.target.value})})]}),(0,v.jsxs)(`label`,{className:`field color-field`,children:[(0,v.jsx)(`span`,{children:`Cor da contagem`}),(0,v.jsx)(`input`,{type:`color`,value:e.countColor||`#ffffff`,onChange:t=>T({...e,countColor:t.target.value})})]}),(0,v.jsxs)(`label`,{className:`field color-field`,children:[(0,v.jsx)(`span`,{children:`Cor do alvo / total`}),(0,v.jsx)(`input`,{type:`color`,value:e.targetColor||`#ffd42a`,onChange:t=>T({...e,targetColor:t.target.value})})]})]}),(0,v.jsxs)(`div`,{className:`button-row compact-row`,children:[(0,v.jsx)(`button`,{className:`secondary-button`,onClick:()=>T({...e,showHeader:!e.showHeader}),children:e.showHeader?`Título visível`:`Título oculto`}),(0,v.jsx)(`button`,{className:`secondary-button`,onClick:()=>T({...e,skipCompleted:!e.skipCompleted}),children:e.skipCompleted?`Pular metas concluídas`:`Mostrar metas concluídas`}),(0,v.jsx)(`button`,{className:`secondary-button`,onClick:M,children:`Restaurar padrão`})]}),(0,v.jsxs)(`div`,{className:`button-row`,children:[(0,v.jsx)(`button`,{className:`secondary-button`,onClick:j,children:`Zerar contagem da live`}),(0,v.jsx)(`button`,{className:`secondary-button`,onClick:()=>navigator.clipboard?.writeText(x),children:`Copiar overlay local`}),(0,v.jsx)(`button`,{className:`secondary-button`,onClick:()=>navigator.clipboard?.writeText(S),children:`Copiar overlay público`})]}),(0,v.jsxs)(`p`,{className:`muted-text`,children:[`URL local: `,(0,v.jsx)(`code`,{children:x})]})]}),(0,v.jsx)(r,{title:`Metas configuradas`,subtitle:`Exemplo: 100 Rosas, 50 Rosquinhas e 5 Galáxias.`,children:(0,v.jsxs)(`div`,{className:`gift-goal-list`,children:[e.goals.length===0?(0,v.jsx)(`p`,{className:`muted-text`,children:`Nenhum gift escolhido ainda. Selecione abaixo até 5 gifts.`}):null,e.goals.map(e=>(0,v.jsxs)(`div`,{className:`gift-goal-row`,children:[(0,v.jsx)(`img`,{src:e.giftImageUrl||`./events/gift.svg`,alt:e.giftName||`Gift`}),(0,v.jsxs)(`div`,{className:`gift-goal-main`,children:[(0,v.jsx)(`strong`,{children:e.giftName||`Gift sem nome`}),(0,v.jsxs)(`div`,{className:`gift-goal-inline-count`,children:[(0,v.jsx)(`strong`,{children:w(e.current)}),(0,v.jsxs)(`span`,{children:[`/ `,w(e.target)]})]}),(0,v.jsx)(`small`,{className:`gift-goal-status`,children:e.enabled?`Meta ativa`:`Meta pausada`})]}),(0,v.jsxs)(`label`,{className:`field small-field`,children:[(0,v.jsx)(`span`,{children:`Meta`}),(0,v.jsx)(E,{value:e.target||1,min:1,max:1e6,step:1,onCommit:t=>k(e.id,{target:t})})]}),(0,v.jsxs)(`div`,{className:`gift-goal-actions`,children:[(0,v.jsx)(`button`,{className:`secondary-button`,onClick:()=>k(e.id,{enabled:!e.enabled}),children:e.enabled?`Meta ativa`:`Meta pausada`}),(0,v.jsx)(`button`,{className:`secondary-button`,onClick:()=>N(e),children:`+1 teste`}),(0,v.jsx)(`button`,{className:`danger-button`,onClick:()=>A(e.id),children:`Remover`})]})]},e.id))]})}),(0,v.jsxs)(r,{title:`Escolher gifts`,subtitle:`Pesquise pelo nome ou valor em moedas.`,children:[(0,v.jsxs)(`label`,{className:`field`,children:[(0,v.jsx)(`span`,{children:`Buscar gift`}),(0,v.jsx)(`input`,{value:n,onChange:e=>p(e.target.value),placeholder:`Rosa, galáxia, rosquinha...`})]}),(0,v.jsx)(`div`,{className:`gift-picker-grid`,children:D.map(t=>(0,v.jsxs)(`button`,{className:`gift-picker-item`,disabled:e.goals.some(e=>e.giftId===t.id)||e.goals.length>=5,onClick:()=>O(t),children:[(0,v.jsx)(`img`,{src:t.imageUrl||`./events/gift.svg`,alt:t.name}),(0,v.jsx)(`strong`,{children:t.name}),(0,v.jsxs)(`small`,{children:[t.coin,` moedas`]})]},t.id))})]}),(0,v.jsx)(`style`,{children:`
        .gift-goal-list { display: grid; gap: 12px; }
        .gift-goal-row { display: grid; grid-template-columns: 64px 1fr 120px auto auto; gap: 12px; align-items: center; padding: 12px; border-radius: 18px; background: rgba(255,255,255,.045); border: 1px solid rgba(255,255,255,.08); }
        .gift-goal-row > img { width: 58px; height: 58px; object-fit: contain; border-radius: 15px; background: rgba(255,255,255,.06); padding: 6px; }
        .gift-goal-main { display: grid; gap: 5px; min-width: 0; }
        .gift-goal-main strong { color: #fff; }
        .gift-goal-main small, .muted-text { color: #aeb9d6; }
        .small-field { margin: 0; }
        .gift-visual-grid { margin-top: 12px; }
        .compact-row { margin-top: 12px; }
        .color-field input[type='color'] { min-height: 40px; padding: 4px; cursor: pointer; }
        .gift-picker-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(132px, 1fr)); gap: 10px; max-height: 520px; overflow: auto; padding-right: 4px; }
        .gift-picker-item { display: grid; place-items: center; gap: 6px; min-height: 132px; border: 1px solid rgba(255,255,255,.08); border-radius: 18px; background: rgba(255,255,255,.045); color: #f8fbff; cursor: pointer; }
        .gift-picker-item:disabled { opacity: .45; cursor: not-allowed; }
        .gift-picker-item img { width: 54px; height: 54px; object-fit: contain; }
        .gift-picker-item small { color: #aeb9d6; }
        @media (max-width: 900px) { .gift-goal-row { grid-template-columns: 54px 1fr; } }
      `})]})}export{D as GiftGoalPage};