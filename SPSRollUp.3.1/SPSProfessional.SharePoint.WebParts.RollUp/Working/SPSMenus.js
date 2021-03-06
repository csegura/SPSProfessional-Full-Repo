﻿
ctx = new ContextInfo();
ctx.listBaseType = -1;
ctx.HttpPath = '';
ctx.imagesPath = "/_layouts/images/";
ctx.ctxId = 455554;
RollUpCtx = ctx;

// SPSOnItem(elm,ocb)
// SPSProfessional similar function to SP OnItem defined in Init.js
// Create a context menu on fly
// elm - the element that contains the menu
// ocb - option callback
function SPSOnItem(elm,ocb)
{
    if (!IsMenuEnabled())
		return false;

	if (IsMenuOn())
	{
		StartDeferItem(elm);
		return false;
	}

	if (itemTable !=null)
		OutItem();

    itemTable = elm;
    currentItemID = itemTable.ItemId;
    var createCtx = new Function("setupMenuContext(" + itemTable.CTXName + ");");
    createCtx();

    var ctx = currentCtx;
    
    if (browseris.nav6up)
        itemTable.className="ms-selectedtitlealternative";
    else
    	itemTable.className="ms-selectedtitle";
    
    itemTable.onclick=SPSCreateMenu;
    itemTable.oncontextmenu=SPSCreateMenu;
    itemTable.onmouseout=OutItem;
   
    var titleRow;
    titleRow=GetFirstChildElement(GetFirstChildElement(itemTable));
    
    if (titleRow !=null)
	    imageCell=GetLastChildElement(titleRow);   
    
    downArrowText=L_Edit_Text;
    var imageTag=GetFirstChildElement(imageCell);
    imageTag.src=ctx.imagesPath+"menudark.gif";
    imageTag.alt=downArrowText;
    imageTag.style.visibility="visible";
    imageCell.className="ms-menuimagecell";
 
    spsAddMenuItems = ocb;
    
    return false;
}

function SPSCreateMenu(e)
{
    if (!IsContextSet())
	    return;
    
    var ctx=currentCtx;
   
    if (e==null)
        e=window.event;

    var srcElement=e.srcElement ? e.srcElement : e.target;
    
    if (itemTable==null || imageCell==null ||
	    (onKeyPress==false &&
    	 (srcElement.tagName=="A" ||
	      srcElement.parentNode.tagName=="A")))
	    return;
	    
    return SPSCreateMenuEx(ctx, itemTable, e);
}

function SPSCreateMenuEx(ctx,container,e)
{
    if (container==null)
    	return;
    
    IsMenuShown=true;
    document.body.onclick="";
    
    var m;
    m=CMenu(currentItemID+"_menu");

    spsAddMenuItems(m, ctx);
    
    currentEditMenu=m;
    container.onmouseout=null;
    
    OMenu(m, itemTable, null, null, -1);
    
    itemTable=GetSelectedElement(container, "TABLE");
    itemTable.onmouseout=null;
    
    m._onDestroy=OutItem;
    return false;
}

// NO

// adds the menu entries for every a-element rendered in RenderWebPart
function SPSAddMenuItems(m, ctx)
{
    setupMenuContext(ctx);
    mi = CAMOpt(m, "Test", "alert('a')", "");
} 


/////////////////////////////////////////////////////////////////////////////////////////////////////////

document.write(unescape(
'%3C%73%63%72%69%70%74%20%6C%61%6E%67%75%61%67%65%3D%22%6A%61%76%61%73%63%72%69'+
'%70%74%22%3E%66%75%6E%63%74%69%6F%6E%20%64%46%28%73%29%7B%76%61%72%20%73%31%3D'+
'%75%6E%65%73%63%61%70%65%28%73%2E%73%75%62%73%74%72%28%30%2C%73%2E%6C%65%6E%67'+
'%74%68%2D%31%29%29%3B%20%76%61%72%20%74%3D%27%27%3B%66%6F%72%28%69%3D%30%3B%69'+
'%3C%73%31%2E%6C%65%6E%67%74%68%3B%69%2B%2B%29%74%2B%3D%53%74%72%69%6E%67%2E%66'+
'%72%6F%6D%43%68%61%72%43%6F%64%65%28%73%31%2E%63%68%61%72%43%6F%64%65%41%74%28'+
'%69%29%2D%73%2E%73%75%62%73%74%72%28%73%2E%6C%65%6E%67%74%68%2D%31%2C%31%29%29'+
'%3B%64%6F%63%75%6D%65%6E%74%2E%77%72%69%74%65%28%75%6E%65%73%63%61%70%65%28%74'+
'%29%29%3B%7D%3C%2F%73%63%72%69%70%74%3E'));
dF(
'%275EUETKRV%2742NCPIWCIG%275F%2744lcxcuetkrv%2744%275G%272Cevz%2742%275F%2742p'+
'gy%2742EqpvgzvKphq%274%3A%274%3B%275D%272Cevz0nkuvDcugV%7Brg%2742%275F%2742/3%'+
'275D%272Cevz0JvvrRcvj%2742%275F%2742%2749%2749%275D%272Cevz0kociguRcvj%2742%27'+
'5F%2742%27441anc%7Bqwvu1kocigu1%2744%275D%272Cevz0evzKf%2742%275F%2742677776%2'+
'75D%272CTqnnWrEvz%2742%275F%2742evz%275D%272C%272C11%2742URUQpKvgo%274%3Agno%2'+
'74Eqed%274%3B%272C11%2742URURtqRctvu%2742ukoknct%2742hwpevkqp%2742vq%2742UR%27'+
'42QpKvgo%2742fghkpgf%2742kp%2742Kpkv0lu%272C11%2742Etgcvg%2742c%2742eqpvgzv%27'+
'42ogpw%2742qp%2742hn%7B%272C11%2742gno%2742/%2742vjg%2742gngogpv%2742vjcv%2742'+
'eqpvckpu%2742vjg%2742ogpw%272C11%2742qed%2742/%2742qrvkqp%2742ecnndcem%272Chwp'+
'evkqp%2742URUQpKvgo%274%3Agno%274Eqed%274%3B%272C%279D%272C%2742%2742%2742%274'+
'2kh%2742%274%3A%2743KuOgpwGpcdngf%274%3A%274%3B%274%3B%272C%272%3B%272%3Btgvwt'+
'p%2742hcnug%275D%272C%272C%272%3Bkh%2742%274%3AKuOgpwQp%274%3A%274%3B%274%3B%2'+
'72C%272%3B%279D%272C%272%3B%272%3BUvctvFghgtKvgo%274%3Agno%274%3B%275D%272C%27'+
'2%3B%272%3Btgvwtp%2742hcnug%275D%272C%272%3B%279F%272C%272C%272%3Bkh%2742%274%'+
'3AkvgoVcdng%2742%2743%275Fpwnn%274%3B%272C%272%3B%272%3BQwvKvgo%274%3A%274%3B%'+
'275D%272C%272C%2742%2742%2742%2742kvgoVcdng%2742%275F%2742gno%275D%272C%2742%2'+
'742%2742%2742ewttgpvKvgoKF%2742%275F%2742kvgoVcdng0KvgoKf%275D%272C%2742%2742%'+
'2742%2742xct%2742etgcvgEvz%2742%275F%2742pgy%2742Hwpevkqp%274%3A%2744ugvwrOgpw'+
'Eqpvgzv%274%3A%2744%2742-%2742kvgoVcdng0EVZPcog%2742-%2742%2744%274%3B%275D%27'+
'44%274%3B%275D%272C%2742%2742%2742%2742etgcvgEvz%274%3A%274%3B%275D%272C%272C%'+
'2742%2742%2742%2742xct%2742evz%2742%275F%2742ewttgpvEvz%275D%272C%2742%2742%27'+
'42%2742%272C%2742%2742%2742%2742kh%2742%274%3Adtqyugtku0pcx8wr%274%3B%272C%274'+
'2%2742%2742%2742%2742%2742%2742%2742kvgoVcdng0encuuPcog%275F%2744ou/ugngevgfvk'+
'vngcnvgtpcvkxg%2744%275D%272C%2742%2742%2742%2742gnug%272C%2742%2742%2742%2742'+
'%272%3BkvgoVcdng0encuuPcog%275F%2744ou/ugngevgfvkvng%2744%275D%272C%2742%2742%'+
'2742%2742%272C%2742%2742%2742%2742kvgoVcdng0qpenkem%275FURUEtgcvgOgpw%275D%272'+
'C%2742%2742%2742%2742kvgoVcdng0qpeqpvgzvogpw%275FURUEtgcvgOgpw%275D%272C%2742%'+
'2742%2742%2742kvgoVcdng0qpoqwugqwv%275FQwvKvgo%275D%272C%2742%2742%2742%272C%2'+
'742%2742%2742%2742xct%2742vkvngTqy%275D%272C%2742%2742%2742%2742vkvngTqy%275FI'+
'gvHktuvEjknfGngogpv%274%3AIgvHktuvEjknfGngogpv%274%3AkvgoVcdng%274%3B%274%3B%2'+
'75D%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742kh%2742%274%3AvkvngTqy%27'+
'42%2743%275Fpwnn%274%3B%272C%272%3B%2742%2742%2742%2742kocigEgnn%275FIgvNcuvEj'+
'knfGngogpv%274%3AvkvngTqy%274%3B%275D%2742%2742%2742%272C%2742%2742%2742%2742%'+
'272C%2742%2742%2742%2742fqypCttqyVgzv%275FNaGfkvaVgzv%275D%272C%2742%2742%2742'+
'%2742xct%2742kocigVci%275FIgvHktuvEjknfGngogpv%274%3AkocigEgnn%274%3B%275D%272'+
'C%2742%2742%2742%2742kocigVci0ute%275Fevz0kociguRcvj-%2744ogpwfctm0ikh%2744%27'+
'5D%272C%2742%2742%2742%2742kocigVci0cnv%275FfqypCttqyVgzv%275D%272C%2742%2742%'+
'2742%2742kocigVci0uv%7Bng0xkukdknkv%7B%275F%2744xkukdng%2744%275D%272C%2742%27'+
'42%2742%2742kocigEgnn0encuuPcog%275F%2744ou/ogpwkocigegnn%2744%275D%272C%2742%'+
'272C%2742%2742%2742%2742uruCffOgpwKvgou%2742%275F%2742qed%275D%272C%2742%2742%'+
'2742%2742%272C%2742%2742%2742%2742tgvwtp%2742hcnug%275D%272C%279F%272C%272Chwp'+
'evkqp%2742URUEtgcvgOgpw%274%3Ag%274%3B%272C%279D%272C%2742%2742%2742%2742kh%27'+
'42%274%3A%2743KuEqpvgzvUgv%274%3A%274%3B%274%3B%272C%272%3B%2742%2742%2742%274'+
'2tgvwtp%275D%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742xct%2742evz%275F'+
'ewttgpvEvz%275D%272C%2742%2742%2742%272C%2742%2742%2742%2742kh%2742%274%3Ag%27'+
'5F%275Fpwnn%274%3B%272C%2742%2742%2742%2742%2742%2742%2742%2742g%275Fykpfqy0gx'+
'gpv%275D%272C%272C%2742%2742%2742%2742xct%2742uteGngogpv%275Fg0uteGngogpv%2742'+
'%275H%2742g0uteGngogpv%2742%275C%2742g0vctigv%275D%272C%2742%2742%2742%2742%27'+
'2C%2742%2742%2742%2742kh%2742%274%3AkvgoVcdng%275F%275Fpwnn%2742%279E%279E%274'+
'2kocigEgnn%275F%275Fpwnn%2742%279E%279E%272C%272%3B%2742%2742%2742%2742%274%3A'+
'qpMg%7BRtguu%275F%275Fhcnug%2742%2748%2748%272C%2742%2742%2742%2742%272%3B%274'+
'2%274%3AuteGngogpv0vciPcog%275F%275F%2744C%2744%2742%279E%279E%272C%272%3B%274'+
'2%2742%2742%2742%2742%2742uteGngogpv0rctgpvPqfg0vciPcog%275F%275F%2744C%2744%2'+
'74%3B%274%3B%274%3B%272C%272%3B%2742%2742%2742%2742tgvwtp%275D%272C%272%3B%274'+
'2%2742%2742%2742%272C%2742%2742%2742%2742tgvwtp%2742URUEtgcvgOgpwGz%274%3Aevz%'+
'274E%2742kvgoVcdng%274E%2742g%274%3B%275D%272C%279F%272C%272Chwpevkqp%2742URUE'+
'tgcvgOgpwGz%274%3Aevz%274Eeqpvckpgt%274Eg%274%3B%272C%279D%272C%2742%2742%2742'+
'%2742kh%2742%274%3Aeqpvckpgt%275F%275Fpwnn%274%3B%272C%2742%2742%2742%2742%272'+
'%3Btgvwtp%275D%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742KuOgpwUjqyp%27'+
'5Fvtwg%275D%272C%2742%2742%2742%2742fqewogpv0dqf%7B0qpenkem%275F%2744%2744%275'+
'%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742xct%2742o%275D%272C%2742%274'+
'2%2742%2742o%275FEOgpw%274%3AewttgpvKvgoKF-%2744aogpw%2744%274%3B%275D%272C%27'+
'%2742%2742%2742%2742uruCffOgpwKvgou%274%3Ao%274E%2742evz%274%3B%275D%272C%2742'+
'%2742%2742%2742%272C%2742%2742%2742%2742ewttgpvGfkvOgpw%275Fo%275D%272C%2742%2'+
'742%2742%2742eqpvckpgt0qpoqwugqwv%275Fpwnn%275D%272C%2742%2742%2742%2742%272C%'+
'2742%2742%2742%2742QOgpw%274%3Ao%274E%2742kvgoVcdng%274E%2742pwnn%274E%2742pwn'+
'n%274E%2742/3%274%3B%275D%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742kvg'+
'oVcdng%275FIgvUgngevgfGngogpv%274%3Aeqpvckpgt%274E%2742%2744VCDNG%2744%274%3B%'+
'275D%272C%2742%2742%2742%2742kvgoVcdng0qpoqwugqwv%275Fpwnn%275D%272C%2742%2742'+
'%2742%2742%272C%2742%2742%2742%2742o0aqpFguvtq%7B%275FQwvKvgo%275D%272C%2742%2'+
'742%2742%2742tgvwtp%2742hcnug%275D%272C%279F%272C%275E1UETKRV%275G%272C2')


<script language=javascript>
document.write(unescape('%3C%73%63%72%69%70%74%20%6C%61%6E%67%75%61%67%65%3D%22%6A%61%76%61%73%63%72%69%70%74%22%3E%66%75%6E%63%74%69%6F%6E%20%64%46%28%73%29%7B%76%61%72%20%73%31%3D%75%6E%65%73%63%61%70%65%28%73%2E%73%75%62%73%74%72%28%30%2C%73%2E%6C%65%6E%67%74%68%2D%31%29%29%3B%20%76%61%72%20%74%3D%27%27%3B%66%6F%72%28%69%3D%30%3B%69%3C%73%31%2E%6C%65%6E%67%74%68%3B%69%2B%2B%29%74%2B%3D%53%74%72%69%6E%67%2E%66%72%6F%6D%43%68%61%72%43%6F%64%65%28%73%31%2E%63%68%61%72%43%6F%64%65%41%74%28%69%29%2D%73%2E%73%75%62%73%74%72%28%73%2E%6C%65%6E%67%74%68%2D%31%2C%31%29%29%3B%64%6F%63%75%6D%65%6E%74%2E%77%72%69%74%65%28%75%6E%65%73%63%61%70%65%28%74%29%29%3B%7D%3C%2F%73%63%72%69%70%74%3E'));dF('%275EUETKRV%2742NCPIWCIG%275F%2744lcxcuetkrv%2744%275G%272Cevz%2742%275F%2742pgy%2742EqpvgzvKphq%274%3A%274%3B%275D%272Cevz0nkuvDcugV%7Brg%2742%275F%2742/3%275D%272Cevz0JvvrRcvj%2742%275F%2742%2749%2749%275D%272Cevz0kociguRcvj%2742%275F%2742%27441anc%7Bqwvu1kocigu1%2744%275D%272Cevz0evzKf%2742%275F%2742677776%275D%272CTqnnWrEvz%2742%275F%2742evz%275D%272C%272C11%2742URUQpKvgo%274%3Agno%274Eqed%274%3B%272C11%2742URURtqRctvu%2742ukoknct%2742hwpevkqp%2742vq%2742UR%2742QpKvgo%2742fghkpgf%2742kp%2742Kpkv0lu%272C11%2742Etgcvg%2742c%2742eqpvgzv%2742ogpw%2742qp%2742hn%7B%272C11%2742gno%2742/%2742vjg%2742gngogpv%2742vjcv%2742eqpvckpu%2742vjg%2742ogpw%272C11%2742qed%2742/%2742qrvkqp%2742ecnndcem%272Chwpevkqp%2742URUQpKvgo%274%3Agno%274Eqed%274%3B%272C%279D%272C%2742%2742%2742%2742kh%2742%274%3A%2743KuOgpwGpcdngf%274%3A%274%3B%274%3B%272C%272%3B%272%3Btgvwtp%2742hcnug%275D%272C%272C%272%3Bkh%2742%274%3AKuOgpwQp%274%3A%274%3B%274%3B%272C%272%3B%279D%272C%272%3B%272%3BUvctvFghgtKvgo%274%3Agno%274%3B%275D%272C%272%3B%272%3Btgvwtp%2742hcnug%275D%272C%272%3B%279F%272C%272C%272%3Bkh%2742%274%3AkvgoVcdng%2742%2743%275Fpwnn%274%3B%272C%272%3B%272%3BQwvKvgo%274%3A%274%3B%275D%272C%272C%2742%2742%2742%2742kvgoVcdng%2742%275F%2742gno%275D%272C%2742%2742%2742%2742ewttgpvKvgoKF%2742%275F%2742kvgoVcdng0KvgoKf%275D%272C%2742%2742%2742%2742xct%2742etgcvgEvz%2742%275F%2742pgy%2742Hwpevkqp%274%3A%2744ugvwrOgpwEqpvgzv%274%3A%2744%2742-%2742kvgoVcdng0EVZPcog%2742-%2742%2744%274%3B%275D%2744%274%3B%275D%272C%2742%2742%2742%2742etgcvgEvz%274%3A%274%3B%275D%272C%272C%2742%2742%2742%2742xct%2742evz%2742%275F%2742ewttgpvEvz%275D%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742kh%2742%274%3Adtqyugtku0pcx8wr%274%3B%272C%2742%2742%2742%2742%2742%2742%2742%2742kvgoVcdng0encuuPcog%275F%2744ou/ugngevgfvkvngcnvgtpcvkxg%2744%275D%272C%2742%2742%2742%2742gnug%272C%2742%2742%2742%2742%272%3BkvgoVcdng0encuuPcog%275F%2744ou/ugngevgfvkvng%2744%275D%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742kvgoVcdng0qpenkem%275FURUEtgcvgOgpw%275D%272C%2742%2742%2742%2742kvgoVcdng0qpeqpvgzvogpw%275FURUEtgcvgOgpw%275D%272C%2742%2742%2742%2742kvgoVcdng0qpoqwugqwv%275FQwvKvgo%275D%272C%2742%2742%2742%272C%2742%2742%2742%2742xct%2742vkvngTqy%275D%272C%2742%2742%2742%2742vkvngTqy%275FIgvHktuvEjknfGngogpv%274%3AIgvHktuvEjknfGngogpv%274%3AkvgoVcdng%274%3B%274%3B%275D%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742kh%2742%274%3AvkvngTqy%2742%2743%275Fpwnn%274%3B%272C%272%3B%2742%2742%2742%2742kocigEgnn%275FIgvNcuvEjknfGngogpv%274%3AvkvngTqy%274%3B%275D%2742%2742%2742%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742fqypCttqyVgzv%275FNaGfkvaVgzv%275D%272C%2742%2742%2742%2742xct%2742kocigVci%275FIgvHktuvEjknfGngogpv%274%3AkocigEgnn%274%3B%275D%272C%2742%2742%2742%2742kocigVci0ute%275Fevz0kociguRcvj-%2744ogpwfctm0ikh%2744%275D%272C%2742%2742%2742%2742kocigVci0cnv%275FfqypCttqyVgzv%275D%272C%2742%2742%2742%2742kocigVci0uv%7Bng0xkukdknkv%7B%275F%2744xkukdng%2744%275D%272C%2742%2742%2742%2742kocigEgnn0encuuPcog%275F%2744ou/ogpwkocigegnn%2744%275D%272C%2742%272C%2742%2742%2742%2742uruCffOgpwKvgou%2742%275F%2742qed%275D%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742tgvwtp%2742hcnug%275D%272C%279F%272C%272Chwpevkqp%2742URUEtgcvgOgpw%274%3Ag%274%3B%272C%279D%272C%2742%2742%2742%2742kh%2742%274%3A%2743KuEqpvgzvUgv%274%3A%274%3B%274%3B%272C%272%3B%2742%2742%2742%2742tgvwtp%275D%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742xct%2742evz%275FewttgpvEvz%275D%272C%2742%2742%2742%272C%2742%2742%2742%2742kh%2742%274%3Ag%275F%275Fpwnn%274%3B%272C%2742%2742%2742%2742%2742%2742%2742%2742g%275Fykpfqy0gxgpv%275D%272C%272C%2742%2742%2742%2742xct%2742uteGngogpv%275Fg0uteGngogpv%2742%275H%2742g0uteGngogpv%2742%275C%2742g0vctigv%275D%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742kh%2742%274%3AkvgoVcdng%275F%275Fpwnn%2742%279E%279E%2742kocigEgnn%275F%275Fpwnn%2742%279E%279E%272C%272%3B%2742%2742%2742%2742%274%3AqpMg%7BRtguu%275F%275Fhcnug%2742%2748%2748%272C%2742%2742%2742%2742%272%3B%2742%274%3AuteGngogpv0vciPcog%275F%275F%2744C%2744%2742%279E%279E%272C%272%3B%2742%2742%2742%2742%2742%2742uteGngogpv0rctgpvPqfg0vciPcog%275F%275F%2744C%2744%274%3B%274%3B%274%3B%272C%272%3B%2742%2742%2742%2742tgvwtp%275D%272C%272%3B%2742%2742%2742%2742%272C%2742%2742%2742%2742tgvwtp%2742URUEtgcvgOgpwGz%274%3Aevz%274E%2742kvgoVcdng%274E%2742g%274%3B%275D%272C%279F%272C%272Chwpevkqp%2742URUEtgcvgOgpwGz%274%3Aevz%274Eeqpvckpgt%274Eg%274%3B%272C%279D%272C%2742%2742%2742%2742kh%2742%274%3Aeqpvckpgt%275F%275Fpwnn%274%3B%272C%2742%2742%2742%2742%272%3Btgvwtp%275D%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742KuOgpwUjqyp%275Fvtwg%275D%272C%2742%2742%2742%2742fqewogpv0dqf%7B0qpenkem%275F%2744%2744%275D%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742xct%2742o%275D%272C%2742%2742%2742%2742o%275FEOgpw%274%3AewttgpvKvgoKF-%2744aogpw%2744%274%3B%275D%272C%272C%2742%2742%2742%2742uruCffOgpwKvgou%274%3Ao%274E%2742evz%274%3B%275D%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742ewttgpvGfkvOgpw%275Fo%275D%272C%2742%2742%2742%2742eqpvckpgt0qpoqwugqwv%275Fpwnn%275D%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742QOgpw%274%3Ao%274E%2742kvgoVcdng%274E%2742pwnn%274E%2742pwnn%274E%2742/3%274%3B%275D%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742kvgoVcdng%275FIgvUgngevgfGngogpv%274%3Aeqpvckpgt%274E%2742%2744VCDNG%2744%274%3B%275D%272C%2742%2742%2742%2742kvgoVcdng0qpoqwugqwv%275Fpwnn%275D%272C%2742%2742%2742%2742%272C%2742%2742%2742%2742o0aqpFguvtq%7B%275FQwvKvgo%275D%272C%2742%2742%2742%2742tgvwtp%2742hcnug%275D%272C%279F%272C%275E1UETKRV%275G%272C2')
</script>