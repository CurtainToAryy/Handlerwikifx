
$(".search-btn").click(function () {
	if ($.trim($('.search-input').val())) {
		location.href = "https://www.wikifx.com/" + "au_en"+"/search.html?keyword=" + encodeURI($.trim($(this).prev().val()));
	} else {
		//$(this).prev().focus();
	}
});
$('.search-input').bind('keypress', function (event) {
	if (event.keyCode == "13") {
		if ($('.search-input').val()) {
		location.href = "https://www.wikifx.com/" + "au_en"+"/search.html?keyword=" + encodeURI($('.search-input').val());
		} else {
			$(this).focus();
		}
	}
});
var vhtmlReg = /^\/.*.html/;
var vhtmlReg1 = /^\/$/;
$(".j-formattime").each(function () {
	$(this).text(moment($(this).text()).add(moment().utcOffset(), 'm').format("YYYY-MM-DD HH:mm"));
});
$(".j-formattime1").each(function () {
	$(this).text(moment($(this).text()).add(moment().utcOffset(), 'm').format("YYYY-MM-DD"));
});

$(".j-formattime2").each(function () {
	var nowtime = moment($(this).text()).add(moment().utcOffset(), 'm');
	//alert(moment().diff(nowtime, 'days'));
	console.log("现在的时间:" + moment().format() + "之前的时间:" + nowtime.format());
	if (moment().diff(nowtime, 'hour') >= 36) {
		$(this).text(nowtime.format("YYYY-MM-DD HH:mm"));
	} else {
		$(this).text(nowtime.fromNow());
	}

});
