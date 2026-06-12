import{r as d,s as N,j as e,C,g as B}from"./index-uZ_ZZ7vJ.js";import{b as y}from"./brazil-gifts-DLMOKDcl.js";import{r as D,a as P,b as _,g as A}from"./overlay-url-IdlNOR_r.js";import{m as S,n as j,a as k,G as u,b as F,c as q}from"./gift-goal-DxayARuM.js";const T="http://127.0.0.1:35942",H="https://liveplay-backend.onrender.com/overlay-bridge/emit";function V(a){const s=String(a||"").trim().replace(/\/$/,"");return s?s.endsWith("/emit")?s:`${s}/emit`:""}function $(a){const s=new Set;return[a,T,H].forEach(n=>{const c=V(n);c&&s.add(c)}),Array.from(s)}function G(a,s){var n,c;if(a.config){try{window.localStorage.setItem(u,JSON.stringify(a.config))}catch{}N(u,a.config).catch(()=>{})}try{const r=new BroadcastChannel(F);r.postMessage(a),r.close()}catch{}typeof((c=(n=window.liveplay)==null?void 0:n.app)==null?void 0:c.emitOverlaySync)=="function"&&window.liveplay.app.emitOverlaySync(a).catch(()=>{});for(const r of $(s))try{fetch(r,{method:"POST",headers:{"Content-Type":"application/json"},body:JSON.stringify(a)}).catch(()=>{})}catch{}}function E(a){return new Intl.NumberFormat("pt-BR").format(Math.max(0,Number(a||0)))}function Y(a,s,n,c){const r=Number(a);return Number.isFinite(r)?Math.max(n,Math.min(c,r)):s}function f({value:a,min:s,max:n,step:c=1,onCommit:r}){const[m,p]=d.useState(String(a));d.useEffect(()=>{p(String(a))},[a]);const b=d.useCallback(()=>{const i=Y(m,a,s,n);p(String(i)),i!==a&&r(i)},[m,n,s,r,a]);return e.jsx("input",{type:"number",min:s,max:n,step:c,value:m,onChange:i=>p(i.target.value),onBlur:b,onKeyDown:i=>{i.key==="Enter"&&i.currentTarget.blur()}})}function Z(){const[a,s]=d.useState(()=>S()),[n,c]=d.useState(""),r=d.useMemo(()=>new URLSearchParams(window.location.search),[]),m=D(r,"/overlay-bridge",T),p=P("/overlay-bridge","giftGoal"),b=_(A(),"/overlay-bridge","giftGoal");d.useEffect(()=>{let t=!0;return(async()=>{const o=await B(u,null).catch(()=>null);t&&o&&s(j(o));try{const x=window.localStorage.getItem(u);t&&x&&s(j(JSON.parse(x)))}catch{}})(),()=>{t=!1}},[]);const i=d.useCallback((t,l="manual")=>{const o=j(t);s(o),G({type:"gift-goal-config",at:Date.now(),config:o,reason:l},m)},[m]);d.useEffect(()=>{var l,o,x;const t=(x=(o=(l=window.liveplay)==null?void 0:l.app)==null?void 0:o.onOverlaySync)==null?void 0:x.call(o,g=>{(g==null?void 0:g.type)==="gift-goal-event"&&g.event&&s(v=>{const h=k(v,g.event);return N(u,h).catch(()=>{}),h}),(g==null?void 0:g.type)==="gift-goal-reset"&&s(v=>{const h=j({...v,goals:v.goals.map(R=>({...R,current:0}))});return N(u,h).catch(()=>{}),h})});return()=>{typeof t=="function"&&t()}},[]);const z=d.useMemo(()=>{const t=n.trim().toLowerCase();return(t?y.filter(o=>`${o.name} ${o.id} ${o.coin}`.toLowerCase().includes(t)):y).slice(0,80)},[n]),M=t=>{if(a.goals.length>=5)return;const l=q();i({...a,goals:[...a.goals,{...l,giftId:t.id,giftName:t.name,giftImageUrl:t.imageUrl||"",accent:t.accent||l.accent}]},"add-goal")},w=(t,l)=>{i({...a,goals:a.goals.map(o=>o.id===t?{...o,...l}:o)},"update-goal")},U=t=>{i({...a,goals:a.goals.filter(l=>l.id!==t)},"remove-goal")},I=()=>{i({...a,goals:a.goals.map(t=>({...t,current:0}))},"reset-counts"),G({type:"gift-goal-reset",at:Date.now(),reason:"manual-reset"},m)},O=()=>{const t=S();i({...a,position:t.position,scale:t.scale,titleSize:t.titleSize,giftSize:t.giftSize,countSize:t.countSize,opacity:t.opacity,skin:t.skin,showHeader:t.showHeader,effect:t.effect,titleColor:t.titleColor,countColor:t.countColor,targetColor:t.targetColor,transition:t.transition,rotateSeconds:t.rotateSeconds,skipCompleted:t.skipCompleted,orderMode:t.orderMode},"restore-visual-defaults")},L=t=>{const l={giftId:t.giftId,giftName:t.giftName,giftImageUrl:t.giftImageUrl,repeatCount:1},o=k(a,l);i(o,"test-event")};return e.jsxs("div",{className:"page-stack",children:[e.jsx("style",{children:`
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
      `}),e.jsxs(C,{title:"Meta de Gifts",subtitle:"Escolha até 5 presentes específicos e mostre o progresso no overlay.",children:[e.jsxs("div",{className:"settings-grid",children:[e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Título do overlay"}),e.jsx("input",{value:a.title,onChange:t=>i({...a,title:t.target.value})})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Status da meta"}),e.jsxs("select",{value:a.enabled?"enabled":"disabled",onChange:t=>i({...a,enabled:t.target.value==="enabled"}),children:[e.jsx("option",{value:"enabled",children:"Ativada"}),e.jsx("option",{value:"disabled",children:"Desativada"})]})]}),e.jsx("div",{className:"gift-goal-enable-row",children:e.jsx("span",{className:"muted-text",children:"Desliga ou liga o overlay inteiro sem apagar as metas configuradas."})}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Posição"}),e.jsxs("select",{value:a.position,onChange:t=>i({...a,position:t.target.value}),children:[e.jsx("option",{value:"center",children:"Centro"}),e.jsx("option",{value:"top-left",children:"Topo esquerdo"}),e.jsx("option",{value:"top-right",children:"Topo direito"}),e.jsx("option",{value:"bottom-left",children:"Baixo esquerdo"}),e.jsx("option",{value:"bottom-right",children:"Baixo direito"})]})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Transição do loop"}),e.jsxs("select",{value:a.transition||"fade",onChange:t=>i({...a,transition:t.target.value}),children:[e.jsx("option",{value:"loop",children:"Loop pulsante"}),e.jsx("option",{value:"fade",children:"Fade"}),e.jsx("option",{value:"slide",children:"Slide"}),e.jsx("option",{value:"flip",children:"Flip 3D"})]})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Tempo de cada gift (s)"}),e.jsx(f,{value:a.rotateSeconds||5,min:2,max:60,step:1,onCommit:t=>i({...a,rotateSeconds:t})})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Ordem do loop"}),e.jsxs("select",{value:a.orderMode||"closest",onChange:t=>i({...a,orderMode:t.target.value}),children:[e.jsx("option",{value:"closest",children:"Mais perto da meta primeiro"}),e.jsx("option",{value:"rotation",children:"Rotação normal"})]})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Efeito extra"}),e.jsxs("select",{value:a.effect||"none",onChange:t=>i({...a,effect:t.target.value}),children:[e.jsx("option",{value:"none",children:"Sem efeito extra"}),e.jsx("option",{value:"neon",children:"Neon LivePlay"}),e.jsx("option",{value:"pulse",children:"Pulsando"}),e.jsx("option",{value:"float",children:"Flutuando"}),e.jsx("option",{value:"pop",children:"Pop no gift"})]})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Estilo"}),e.jsxs("select",{value:a.skin,onChange:t=>i({...a,skin:t.target.value}),children:[e.jsx("option",{value:"neon",children:"Sem fundo / neon"}),e.jsx("option",{value:"clean",children:"Clean"}),e.jsx("option",{value:"glass",children:"Glass discreto"})]})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Tamanho do overlay"}),e.jsx(f,{value:a.scale||1,min:.4,max:2.4,step:.05,onCommit:t=>i({...a,scale:t})})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Tamanho do título"}),e.jsx(f,{value:a.titleSize||36,min:16,max:96,step:1,onCommit:t=>i({...a,titleSize:t})})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Tamanho do gift"}),e.jsx(f,{value:a.giftSize||180,min:64,max:420,step:1,onCommit:t=>i({...a,giftSize:t})})]}),e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Tamanho dos números"}),e.jsx(f,{value:a.countSize||48,min:20,max:120,step:1,onCommit:t=>i({...a,countSize:t})})]})]}),e.jsxs("div",{className:"settings-grid gift-visual-grid",children:[e.jsxs("label",{className:"field color-field",children:[e.jsx("span",{children:"Cor do título"}),e.jsx("input",{type:"color",value:a.titleColor||"#ffffff",onChange:t=>i({...a,titleColor:t.target.value})})]}),e.jsxs("label",{className:"field color-field",children:[e.jsx("span",{children:"Cor da contagem"}),e.jsx("input",{type:"color",value:a.countColor||"#ffffff",onChange:t=>i({...a,countColor:t.target.value})})]}),e.jsxs("label",{className:"field color-field",children:[e.jsx("span",{children:"Cor do alvo / total"}),e.jsx("input",{type:"color",value:a.targetColor||"#ffd42a",onChange:t=>i({...a,targetColor:t.target.value})})]})]}),e.jsxs("div",{className:"button-row compact-row",children:[e.jsx("button",{className:"secondary-button",onClick:()=>i({...a,showHeader:!a.showHeader}),children:a.showHeader?"Título visível":"Título oculto"}),e.jsx("button",{className:"secondary-button",onClick:()=>i({...a,skipCompleted:!a.skipCompleted}),children:a.skipCompleted?"Pular metas concluídas":"Mostrar metas concluídas"}),e.jsx("button",{className:"secondary-button",onClick:O,children:"Restaurar padrão"})]}),e.jsxs("div",{className:"button-row",children:[e.jsx("button",{className:"secondary-button",onClick:I,children:"Zerar contagem da live"}),e.jsx("button",{className:"secondary-button",onClick:()=>{var t;return(t=navigator.clipboard)==null?void 0:t.writeText(p)},children:"Copiar overlay local"}),e.jsx("button",{className:"secondary-button",onClick:()=>{var t;return(t=navigator.clipboard)==null?void 0:t.writeText(b)},children:"Copiar overlay público"})]}),e.jsxs("p",{className:"muted-text",children:["URL local: ",e.jsx("code",{children:p})]})]}),e.jsx(C,{title:"Metas configuradas",subtitle:"Exemplo: 100 Rosas, 50 Rosquinhas e 5 Galáxias.",children:e.jsxs("div",{className:"gift-goal-list",children:[a.goals.length===0?e.jsx("p",{className:"muted-text",children:"Nenhum gift escolhido ainda. Selecione abaixo até 5 gifts."}):null,a.goals.map(t=>e.jsxs("div",{className:"gift-goal-row",children:[e.jsx("img",{src:t.giftImageUrl||"./events/gift.svg",alt:t.giftName||"Gift"}),e.jsxs("div",{className:"gift-goal-main",children:[e.jsx("strong",{children:t.giftName||"Gift sem nome"}),e.jsxs("div",{className:"gift-goal-inline-count",children:[e.jsx("strong",{children:E(t.current)}),e.jsxs("span",{children:["/ ",E(t.target)]})]}),e.jsx("small",{className:"gift-goal-status",children:t.enabled?"Meta ativa":"Meta pausada"})]}),e.jsxs("label",{className:"field small-field",children:[e.jsx("span",{children:"Meta"}),e.jsx(f,{value:t.target||1,min:1,max:1e6,step:1,onCommit:l=>w(t.id,{target:l})})]}),e.jsxs("div",{className:"gift-goal-actions",children:[e.jsx("button",{className:"secondary-button",onClick:()=>w(t.id,{enabled:!t.enabled}),children:t.enabled?"Meta ativa":"Meta pausada"}),e.jsx("button",{className:"secondary-button",onClick:()=>L(t),children:"+1 teste"}),e.jsx("button",{className:"danger-button",onClick:()=>U(t.id),children:"Remover"})]})]},t.id))]})}),e.jsxs(C,{title:"Escolher gifts",subtitle:"Pesquise pelo nome ou valor em moedas.",children:[e.jsxs("label",{className:"field",children:[e.jsx("span",{children:"Buscar gift"}),e.jsx("input",{value:n,onChange:t=>c(t.target.value),placeholder:"Rosa, galáxia, rosquinha..."})]}),e.jsx("div",{className:"gift-picker-grid",children:z.map(t=>{const l=a.goals.some(o=>o.giftId===t.id);return e.jsxs("button",{className:"gift-picker-item",disabled:l||a.goals.length>=5,onClick:()=>M(t),children:[e.jsx("img",{src:t.imageUrl||"./events/gift.svg",alt:t.name}),e.jsx("strong",{children:t.name}),e.jsxs("small",{children:[t.coin," moedas"]})]},t.id)})})]}),e.jsx("style",{children:`
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
      `})]})}export{Z as GiftGoalPage};
