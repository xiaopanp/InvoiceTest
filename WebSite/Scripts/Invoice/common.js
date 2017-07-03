
var __execflag = 0;

function getRdpPostURL(actionPage)
{
	var pathname = window.location.pathname;
	var searchParams = window.location.search;
	if (searchParams == null || searchParams.length<1)
	{
		searchParams="?__execflag="+__execflag;
	}else
	{
		searchParams += "&__execflag="+__execflag;
	}

	if (pathname.indexOf("View.htm")>0)
	{
		actionPage = "View.htm/"+actionPage;
	}
	
	if (pathname.indexOf(".htm")<0)
	{
		actionPage = "bizService/"+actionPage;
	}

	var theUrl = actionPage+searchParams;
	__execflag++;

	return theUrl;
}

/**
 *刷新子模板方法
 *divId:子模板的var值
 *reqParam:刷新参数
 *append:递增刷新
 */
function refreshChildTemplate(divId,reqParam,append,sc)
{
	__execflag++;
	if ($('#'+divId) == null) return false;
	var refData = $('#'+divId).attr("refData");
	var refTemplateId = $('#'+divId).attr("refTemplateId");
	var url = getRdpPostURL("refreshTemplate.htm");
	
	var params = {};
	if (reqParam == null)
	{
		reqParam = {};
	}

	//循环将增量变量写入到子模板状态中
	var refDataStr = $('#'+divId).attr("refData");
	var refDataPage = $.parseJSON(refDataStr);

	$.each(reqParam,function(name,value) { 
		refDataPage[name] = value;
	}); 

	$('#'+divId).attr("refData",JSON.stringify(refDataPage));
	//新增变量写入结束

	params.refData = decodeURI(refData);
	params.reqParam = JSON.stringify(reqParam);
	params.refTemplateId = refTemplateId;

	if (rdpLocal != null)
	{
		rdpLocal.startBusy();
	}
	
	$.ajax({
		type : "post",
		datatype : "json",
		data:params,
		url :url,
		async : true,
		success : function(responsedata) {
			if (rdpLocal != null)
			{
				rdpLocal.stopBusy();
			}
			var rspData = $.parseJSON(responsedata);
			if (append)
			{
				if (sc != null)
				{
					sc(rspData);
				}
			}else
			{
				$('#'+divId).html('');
				//$('#'+divId).append(rspData);
				
				$('#'+divId).html(rspData);
				if (sc != null)
				{
					sc(rspData);
				}
			}
			//自动处理子模板的预初始化
			rdpTemplateInit();
		},
		error : function(XMLHttpRequest, textStatus, errorThrown){
			//alert("访问出错");
		}
	});
}

/**
 *AJAX切换页方法
 *divId:子模板的var值
 *tagVar:刷数数据变量对应的var值，也即是变量标签的var值
 *pageNo:页码
 */
function gotoChildPage(divId,tagVar,pageNo)
{
	var paramname = "cpno";
	if (tagVar != null && tagVar.length>0)
	{
		paramname = tagVar+"_cpno";
	}
	
	var reqParam = {};

	reqParam[paramname] = pageNo;

	refreshChildTemplate(divId,reqParam);
}


/**
 *AJAX更多记录方法
 *divId:子模板的var值
 *tagVar:刷数数据变量对应的var值，也即是变量标签的var值
 */
function addMorePage(divId,tagVar)
{
	if ($('#'+divId) == null) return false;
	var refDataStr = $('#'+divId).attr("refData");

	var refData = $.parseJSON(refDataStr);

	var pageNoId = "cpno";
	if (tagVar != null && tagVar.length>0)
	{
		pageNoId = tagVar+"_cpno";
	}
	

	var thePageNo = refData[pageNoId];

	if (thePageNo == null || thePageNo.length<1)
	{
		thePageNo = "1";
	}

	var newPageNo = parseInt(thePageNo) +1;
	
	refData[pageNoId] = newPageNo;
	
	$('#'+divId).attr("refData",JSON.stringify(refData));
	
	var reqParam = {};

	reqParam[pageNoId] = newPageNo;

	refreshChildTemplate(divId,reqParam,true);
}

/**
 *执行业务服务
 *businessURL:业务调用根地址
 *bisinessNo:业务服务号
 *params:json参数
 *sc:回调函数
 */
function businessExec(bisinessNo,params,sc,isSync)
{
	params = dataEncode(params);
	if(isSync==null){
		isSync=true;
	}
	//generatSign(bisinessNo,params);
	var url = getRdpPostURL(bisinessNo+"/biz-service.htm");
	if(url.indexOf("?")>-1){
		var a=url.indexOf("?");
		url=url.substr(0,a);
	}
	
	if(params != null && params.method != "setSessionValues" && params.method != "getJSTicket")
	{
		clearHistory();
	}
	$.ajax({
			type : "post",
			datatype : "json",
			data:params,
			url :url,
			async : isSync,
			success : function(responsedata) {
				var rspData = $.parseJSON(responsedata);
				rspData=dataEncodeOut(rspData);
				if (rspData == null)
				{
					alert("业务调用失败！");
					return;
				}

				if (rspData.RESULT == "-1")
				{
					alert("业务调用失败！");
					return;
				}else if (rspData.RESULT == "2")
				{
					alert("站点不存在！");
					return;
				}else if (rspData.RESULT == "3")
				{
					alert("业务号不存在！");
					return;
				}else if (rspData.RESULT == "4")
				{
					alert("业务服务不存在！");
					return;
				}else if (rspData.RESULT == "5")
				{
					alert(rspData.EXCEPTION);
					return;
				}else if (rspData.RESULT == "1001")
				{
					if(rspData.EXCEPTION=="Incorrent Password"){
						alert("老密码输入错误");
						return;
					}else{
						alert(rspData.EXCEPTION);
						return;
					}
				}
				if (rspData.RESULT != null)
				{
					if(params != null && params.method != "setSessionValues" && params.method != "getJSTicket")
					{
						clearHistory();
					}
					//执行成功，调用回调函数
					if(sc != null)
					{
						sc(rspData.DATA,rspData);

						return;
					}
				}
				return;
			},
			error : function(XMLHttpRequest, textStatus, errorThrown){
				//alert("访问出错");
			}
		});

}

/**
 *执行业务服务
 *businessURL:业务调用根地址
 *bisinessNo:业务服务号
 *params:json参数
 *sc:回调函数
 *timeOutSc:超时回调函数
 *timeOut:超时时间
 */
function businessExecTimeOut(bisinessNo,params,sc,timeOutSc,timeOut,isSync)
{
	params = dataEncode(params);
	if(isSync==null){
		isSync=true;
	}
	if(timeOut==null){
		timeOut=0;
	}
	
	//generatSign(bisinessNo,params);
	var url = getRdpPostURL(bisinessNo+"/biz-service.htm");
	if(url.indexOf("?")>-1){
		var a=url.indexOf("?");
		url=url.substr(0,a);
	}
	
	if(params != null && params.method != "setSessionValues" && params.method != "getJSTicket")
	{
		clearHistory();
	}
	$.ajax({
			type : "post",
			datatype : "json",
			data:params,
			url :url,
			timeout :timeOut,
			async : isSync,
			success : function(responsedata) {
				var rspData = $.parseJSON(responsedata);
				rspData=dataEncodeOut(rspData);
				if (rspData == null)
				{
					alert("业务调用失败！");
					return;
				}

				if (rspData.RESULT == "-1")
				{
					alert("业务调用失败！");
					return;
				}else if (rspData.RESULT == "2")
				{
					alert("站点不存在！");
					return;
				}else if (rspData.RESULT == "3")
				{
					alert("业务号不存在！");
					return;
				}else if (rspData.RESULT == "4")
				{
					alert("业务服务不存在！");
					return;
				}else if (rspData.RESULT == "5")
				{
					alert(rspData.EXCEPTION);
					return;
				}else if (rspData.RESULT == "1001")
				{
					if(rspData.EXCEPTION=="Incorrent Password"){
						alert("老密码输入错误");
						return;
					}else{
						alert(rspData.EXCEPTION);
						return;
					}
				}
				if (rspData.RESULT != null)
				{
					if(params != null && params.method != "setSessionValues" && params.method != "getJSTicket")
					{
						clearHistory();
					}
					//执行成功，调用回调函数
					if(sc != null)
					{
						sc(rspData.DATA,rspData);

						return;
					}
				}
				return;
			},
			error : function(XMLHttpRequest, textStatus, errorThrown){
				//alert("访问出错");
			},
			　　complete : function(XMLHttpRequest,status){ //请求完成后最终执行参数
			　　　　if(status=='timeout'||status=='error'){//超时,status还有success,error等值的情况
			　　　　　  timeOutSc();
			　　　　}
			　　}
		});

}

//判断客户端是否是微信
function isWeiXin(){
    var ua = window.navigator.userAgent.toLowerCase();
    if(ua.match(/MicroMessenger/i) == 'micromessenger'){
        return true;
    }else{
		if (ua.indexOf('windows phone')>=0 && ua.indexOf('compatible') >=0)
		{
			//return true;
		}
        return false;
    }
}

//清除滚动缓存
function clearHistory()
{
	var currentTime = new Date().getTime()+500;
	window.localStorage.setItem("ALL_HISTORY_TIME",currentTime);
}

function htmlEncode (str){
    var s = "";
    if (str.length == 0) return "";
    //s = str.replace(/ /g, "&nbsp;");
    //s = str.replace(/&/g, "&amp;");
    s = str.replace(/</g, "%26lt%3B");
    s=s.replace(/%3C/g,"%26lt%3B");
    s=s.replace(/%3c/g,"%26lt%3B");
    s = s.replace(/>/g, "%26gt%3B");
    s = s.replace(/%3E/g, "%26gt%3B");
    s = s.replace(/%3e/g, "%26gt%3B");
    //s = s.replace(/\'/g, "&#39;");
    //s = s.replace(/\"/g, "&quot;");
    //s = s.replace(/\n/g, "<br>");
    return s;
};
function htmlEncodeOut (str){
    var s = "";
    if (str.length == 0) return "";
    //s = str.replace(/ /g, "&nbsp;");
    //s = str.replace(/&/g, "&amp;");
    s = str.replace(/</g, "&lt;");
    s=s.replace(/%3C/g,"&lt;");
    s=s.replace(/%3c/g,"&lt;");
    s = s.replace(/>/g, "&gt;");
    s = s.replace(/%3E/g, "&gt;");
    s = s.replace(/%3e/g, "&gt;");
    s = s.replace(/%26lt%3B/g, "&lt;");
    s = s.replace(/%26lt%3b/g, "&lt;");
    s = s.replace(/%26gt%3B/g, "&gt;");
    s = s.replace(/%26gt%3b/g, "&gt;");
    //s = s.replace(/\'/g, "&#39;");
    //s = s.replace(/\"/g, "&quot;");
    //s = s.replace(/\n/g, "<br>");
    return s;
};
function dataEncode(data){
    var rel=data;
    var source="";
    if(typeof(rel) == "object"){
        source=htmlEncode(JSON.stringify(rel));
        source=JSON.parse(source);
        rel=source;
    }else if(typeof(rel) == "string"){
        source=htmlEncode(rel);
        rel=source;
    }
    return rel;
};
function dataEncodeOut(data){
    var rel=data;
    var source="";
    if(typeof(rel) == "object"){
        source=htmlEncodeOut(JSON.stringify(rel));
        source=JSON.parse(source);
        rel=source;
    }else if(typeof(rel) == "string"){
        source=htmlEncodeOut(rel);
        rel=source;
    }
    return rel;
};