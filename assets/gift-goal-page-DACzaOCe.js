import{r as d,i as j,j as e,C as v,f as L}from"./index-0uk_KvXQ.js";import{b as w}from"./brazil-gifts-CAp08RMG.js";import{r as R,b as B,a as P,g as _}from"./overlay-url-yJYaczpp.js";import{m as A,n as x,b as y,G as p,a as D,c as F}from"./gift-goal-ByACYXf6.js";const k="http://127.0.0.1:35942",z="https://liveplay-backend.onrender.com/overlay-bridge/emit";function q(a){const s=String(a||"").trim().replace(/\/$/,"");return s?s.endsWith("/emit")?s:`${s}/emit`:""}function $(a){const s=new Set;return[a,k,z].forEach(n=>{const c=q(n);c&&s.add(c)}),Array.from(s)}function C(a,s){var n,c;if(a.config){try{window.localStorage.setItem(p,JSON.stringify(a.config))}catch{}j(p,a.config).catch(()=>{})}try{const g=new BroadcastChannel(D);g.postMessage(a),g.close()}catch{}typeof((c=(n=window.liveplay)==null?void 0:n.app)==null?void 0:c.emitOverlaySync)=="function"&&window.liveplay.app.emitOverlaySync(a).catch(()=>{});for(const g of $(s))try{fetch(g,{method:"POST",headers:{"Content-Type":"application/json"},body:JSON.stringify(a)}).catch(()=>{})}catch{}}function G(a){return new Intl.NumberFormat("pt-BR").format(Math.max(0,Number(a||0)))}function K(){const[a,s]=d.useState(()=>A()),[n,c]=d.useState(""),g=d.useMemo(()=>new URLSearchParams(window.location.search),[]),h=R(g,"/overlay-bridge",k),b=B("/overlay-bridge","giftGoal"),S=P(_(),"/overlay-bridge","giftGoal");d.useEffect(()=>{let t=!0;return(async()=>{const i=await L(p,null).catch(()=>null);t&&i&&s(x(i));try{const m=window.localStorage.getItem(p);t&&m&&s(x(JSON.parse(m)))}catch{}})(),()=>{t=!1}},[]);const l=d.useCallback((t,o="manual")=>{const i=x(t);s(i),C({type:"gift-goal-config",at:Date.now(),config:i,reason:o},h)},[h]);d.useEffect(()=>{var o,i,m;const t=(m=(i=(o=window.liveplay)==null?void 0:o.app)==null?void 0:i.onOverlaySync)==null?void 0:m.call(i,r=>{(r==null?void 0:r.type)==="gift-goal-event"&&r.event&&s(u=>{const f=y(u,r.event);return j(p,f).catch(()=>{}),f}),(r==null?void 0:r.type)==="gift-goal-reset"&&s(u=>{const f=x({...u,goals:u.goals.map(M=>({...M,current:0}))});return j(p,f).catch(()=>{}),f})});return()=>{typeof t=="function"&&t()}},[]);const E=d.useMemo(()=>{const t=n.trim().toLowerCase();return(t?w.filter(i=>`${i.name} ${i.id} ${i.coin}`.toLowerCase().includes(t)):w).slice(0,80)},[n]),U=t=>{if(a.goals.length>=5)return;const o=F();l({...a,goals:[...a.goals,{...o,giftId:t.id,giftName:t.name,giftImageUrl:t.imageUrl||"",accent:t.accent||o.accent}]},"add-goal")},N=(t,o)=>{l({...a,goals:a.goals.map(i=>i.id===t?{...i,...o}:i)},"update-goal")},I=t=>{l({...a,goals:a.goals.filter(o=>o.id!==t)},"remove-goal")},T=()=>{l({...a,goals:a.goals.map(t=>({...t,current:0}))},"reset-counts"),C({type:"gift-goal-reset",at:Date.now(),reason:"manual-reset"},h)},O=t=>{const o={giftId:t.giftId,giftName:t.giftName,giftImageUrl:t.giftImageUrl,repeatCount:1},i=y(a,o);l(i,"test-event")};return e.jsxs("div",{className:"page-stack",children:[e.jsx("style",{children:`
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
      `}),e.jsxs(v,{title:"Meta de Gifts",subtitle:"Escolha até 5 presentes específicos e mostre o progresso no overlay.",children:[e.jsxs("div",{className:"settings-grid",children:[e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Título do overlay"}),e.jsx("input",{value:a.title,onChange:t=>l({...a,title:t.target.value})})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Status da meta"}),e.jsxs("select",{value:a.enabled?"enabled":"disabled",onChange:t=>l({...a,enabled:t.target.value==="enabled"}),children:[e.jsx("option",{value:"enabled",children:"Ativada"}),e.jsx("option",{value:"disabled",children:"Desativada"})]})]}),e.jsx("div",{className:"gift-goal-enable-row",children:e.jsx("span",{className:"muted-text",children:"Desliga ou liga o overlay inteiro sem apagar as metas configuradas."})}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Posição"}),e.jsxs("select",{value:a.position,onChange:t=>l({...a,position:t.target.value}),children:[e.jsx("option",{value:"center",children:"Centro"}),e.jsx("option",{value:"top-left",children:"Topo esquerdo"}),e.jsx("option",{value:"top-right",children:"Topo direito"}),e.jsx("option",{value:"bottom-left",children:"Baixo esquerdo"}),e.jsx("option",{value:"bottom-right",children:"Baixo direito"})]})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Transição do loop"}),e.jsxs("select",{value:a.transition||"fade",onChange:t=>l({...a,transition:t.target.value}),children:[e.jsx("option",{value:"loop",children:"Loop pulsante"}),e.jsx("option",{value:"fade",children:"Fade"}),e.jsx("option",{value:"slide",children:"Slide"}),e.jsx("option",{value:"flip",children:"Flip 3D"})]})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Tempo de cada gift (s)"}),e.jsx("input",{type:"number",min:2,max:60,value:a.rotateSeconds||5,onChange:t=>l({...a,rotateSeconds:Number(t.target.value)})})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Ordem do loop"}),e.jsxs("select",{value:a.orderMode||"closest",onChange:t=>l({...a,orderMode:t.target.value}),children:[e.jsx("option",{value:"closest",children:"Mais perto da meta primeiro"}),e.jsx("option",{value:"rotation",children:"Rotação normal"})]})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Efeito extra"}),e.jsxs("select",{value:a.effect||"none",onChange:t=>l({...a,effect:t.target.value}),children:[e.jsx("option",{value:"none",children:"Sem efeito extra"}),e.jsx("option",{value:"neon",children:"Neon LivePlay"}),e.jsx("option",{value:"pulse",children:"Pulsando"}),e.jsx("option",{value:"float",children:"Flutuando"}),e.jsx("option",{value:"pop",children:"Pop no gift"})]})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Estilo"}),e.jsxs("select",{value:a.skin,onChange:t=>l({...a,skin:t.target.value}),children:[e.jsx("option",{value:"neon",children:"Sem fundo / neon"}),e.jsx("option",{value:"clean",children:"Clean"}),e.jsx("option",{value:"glass",children:"Glass discreto"})]})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Tamanho do overlay"}),e.jsx("input",{type:"number",min:.4,max:2.4,step:"0.05",value:a.scale,onChange:t=>l({...a,scale:Number(t.target.value)})})]})]}),e.jsxs("div",{className:"settings-grid gift-visual-grid",children:[e.jsxs("label",{className:"field color-field",children:[e.jsx("span",{children:"Cor do título"}),e.jsx("input",{type:"color",value:a.titleColor||"#ffffff",onChange:t=>l({...a,titleColor:t.target.value})})]}),e.jsxs("label",{className:"field color-field",children:[e.jsx("span",{children:"Cor da contagem"}),e.jsx("input",{type:"color",value:a.countColor||"#ffffff",onChange:t=>l({...a,countColor:t.target.value})})]}),e.jsxs("label",{className:"field color-field",children:[e.jsx("span",{children:"Cor do alvo / total"}),e.jsx("input",{type:"color",value:a.targetColor||"#ffd42a",onChange:t=>l({...a,targetColor:t.target.value})})]})]}),e.jsxs("div",{className:"button-row compact-row",children:[e.jsx("button",{className:"secondary-button",onClick:()=>l({...a,showHeader:!a.showHeader}),children:a.showHeader?"Título visível":"Título oculto"}),e.jsx("button",{className:"secondary-button",onClick:()=>l({...a,skipCompleted:!a.skipCompleted}),children:a.skipCompleted?"Pular metas concluídas":"Mostrar metas concluídas"})]}),e.jsxs("div",{className:"button-row",children:[e.jsx("button",{className:"secondary-button",onClick:T,children:"Zerar contagem da live"}),e.jsx("button",{className:"secondary-button",onClick:()=>{var t;return(t=navigator.clipboard)==null?void 0:t.writeText(b)},children:"Copiar overlay local"}),e.jsx("button",{className:"secondary-button",onClick:()=>{var t;return(t=navigator.clipboard)==null?void 0:t.writeText(S)},children:"Copiar overlay público"})]}),e.jsxs("p",{className:"muted-text",children:["URL local: ",e.jsx("code",{children:b})]})]}),e.jsx(v,{title:"Metas configuradas",subtitle:"Exemplo: 100 Rosas, 50 Rosquinhas e 5 Galáxias.",children:e.jsxs("div",{className:"gift-goal-list",children:[a.goals.length===0?e.jsx("p",{className:"muted-text",children:"Nenhum gift escolhido ainda. Selecione abaixo até 5 gifts."}):null,a.goals.map(t=>e.jsxs("div",{className:"gift-goal-row",children:[e.jsx("img",{src:t.giftImageUrl||"./events/gift.svg",alt:t.giftName||"Gift"}),e.jsxs("div",{className:"gift-goal-main",children:[e.jsx("strong",{children:t.giftName||"Gift sem nome"}),e.jsxs("div",{className:"gift-goal-inline-count",children:[e.jsx("strong",{children:G(t.current)}),e.jsxs("span",{children:["/ ",G(t.target)]})]}),e.jsx("small",{className:"gift-goal-status",children:t.enabled?"Meta ativa":"Meta pausada"})]}),e.jsxs("label",{className:"field small-field",children:[e.jsx("span",{children:"Meta"}),e.jsx("input",{type:"number",min:1,value:t.target,onChange:o=>N(t.id,{target:Number(o.target.value)})})]}),e.jsxs("div",{className:"gift-goal-actions",children:[e.jsx("button",{className:"secondary-button",onClick:()=>N(t.id,{enabled:!t.enabled}),children:t.enabled?"Meta ativa":"Meta pausada"}),e.jsx("button",{className:"secondary-button",onClick:()=>O(t),children:"+1 teste"}),e.jsx("button",{className:"danger-button",onClick:()=>I(t.id),children:"Remover"})]})]},t.id))]})}),e.jsxs(v,{title:"Escolher gifts",subtitle:"Pesquise pelo nome ou valor em moedas.",children:[e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Buscar gift"}),e.jsx("input",{value:n,onChange:t=>c(t.target.value),placeholder:"Rosa, galáxia, rosquinha..."})]}),e.jsx("div",{className:"gift-picker-grid",children:E.map(t=>{const o=a.goals.some(i=>i.giftId===t.id);return e.jsxs("button",{className:"gift-picker-item",disabled:o||a.goals.length>=5,onClick:()=>U(t),children:[e.jsx("img",{src:t.imageUrl||"./events/gift.svg",alt:t.name}),e.jsx("strong",{children:t.name}),e.jsxs("small",{children:[t.coin," moedas"]})]},t.id)})})]}),e.jsx("style",{children:`
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
      `})]})}export{K as GiftGoalPage};
