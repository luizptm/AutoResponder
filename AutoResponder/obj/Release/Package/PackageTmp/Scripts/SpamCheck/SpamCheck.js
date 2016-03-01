var SpamCheck = {

	init: function(){
		Cufon.replace('#api h1 span, #doc article h1');

		SpamCheck.iefix();
		SpamCheck.realign();

		$("#check form").submit(function(e) {
			e.preventDefault();
			SpamCheck.runCheck();
		});

		$(document).keyup(function(e) {
			if (e.keyCode == 27) { 
				$('#output a.close').click();
			}
		});
	},

	iefix: function() {

	    if ($.browser && $.browser.msie && parseInt($.browser.version, 10) < 9) {
			$('.wrap').height() < 650 ? $('.wrap-beta').height('650px') : $('.wrap-beta').height('100%');
		}

	},
	
	realign: function() {

		if ($('#doc').length != 0) return;

		$('body').removeClass('narrowscreen tinyscreen smallscreen');

		if ($('.wrap').width() < 1110) {
			$('body').addClass('narrowscreen');
		}

		if ($('.wrap').height() < 790) {
			$('body').addClass('tinyscreen');
			$('#check textarea').height('100px');
			$('#main, #output').css('top', '11%');
		}
		else {

			// 790 ... 920
			if ($('.wrap').height() < 920) {
				$('body').addClass('smallscreen');
				$('#check textarea').height('100px');
			
			}
			// 920 ... 1070
			else if ($('.wrap').height() < 1070) {
				$('body').addClass('smallscreen');
				$('#check textarea').height('200px');
			}
			// 1070...
			else {
				$('#check textarea').height(function() {
					return $('#api').height() + ($(this).outerHeight(true) - $(this).height()) * -1;
				});
			}

			$('#main, #output').css('top', function() {
				return (($('.wrap').height() - $(this).innerHeight()) / 2) + 'px';
			});
		}


		Cufon.refresh();

	},
	
	runCheck: function() {

		// Make sure its not empty
		var email = $("#check textarea").val();
		if ($('#check .error').length != 0) $('#check .error').remove();

		if (email == '') {

			$('#check').prepend('<div class="error" style="display:none;" />');
			$('#check .error').html('No email — no spam. Please paste your message.');
			$('#check .error').animate({height: 'show'}, 100);

			return false;
		}

		// Run check

		$("#score span.unknown").addClass('hide');
		$("#score span.value").addClass('hide');
		$("#score span.loading").removeClass('hide');

		$.ajax({
			url: "http://spamcheck.postmarkapp.com/filter",
			data: { 'email': $("#check textarea").val(), 'options': 'long' }, type: 'POST', crossDomain: true, dataType: 'jsonp',
			success: function () {
				$("#score span.loading").addClass('hide');
				$("#score span.value").removeClass('hide');

				if ("score" in data) {
					SpamCheck.setScale(data.score.split('/')[0]);
					if ("report" in data) {
						if ($.browser.msie && parseInt($.browser.version, 10) <= 7) {
							data.report = data.report.replace(/\n/g, '\r');
						}
						$("#output code").text(data.report);
						SpamCheck.output();
					}
				}
				else if ("message" in data) {
					//TODO: replace this with prettiness.
					alert(data.message);
				}
			}
		});
		/*
		$.post("http://spamcheck.postmarkapp.com/filter", { 'email': $("#check textarea").val(), 'options': 'long' }, function (data) {
			$("#score span.loading").addClass('hide');
			$("#score span.value").removeClass('hide');

			if ("score" in data) {
				SpamCheck.setScale(data.score.split('/')[0]);
				if ("report" in data) {
					if ($.browser.msie && parseInt($.browser.version, 10) <= 7) { 
						data.report = data.report.replace(/\n/g, '\r'); 
					}
					$("#output code").text(data.report);
					SpamCheck.output();
				}
			}
			else if("message" in data) {
				//TODO: replace this with prettiness.
				alert(data.message);
			}
		});
		*/
	},

	setScale: function(score) {
		var roundedScore = Math.floor(score);

		$("#score span.value").addClass('s'+roundedScore);
		$("#score span.value s").html(score);

		if (roundedScore <= 0) {
			var scoreDesc = 'Excellent';
		}
		else if (roundedScore > 0 && roundedScore < 5) {
			var scoreDesc = 'Good';
		}
		else {
			var scoreDesc = '<a href="http://www.youtube.com/watch?v=anwy2MPT5RE">Spam</a>';
		}
		$("#score span.value b").html(scoreDesc);

 	},
	
	output: function() {

		SpamCheck.realign();
		$('#main').animate({left: '-=100%'});
		$('#output').animate({left: '+=100%'});

		$('#output a.close').click(function(e) {
			e.preventDefault();

			$('#main').animate({left: '+=100%'});
			$('#output').animate({left: '-=100%'});

			$(this).unbind('click');
		});
	}
}

$(document).ready(SpamCheck.init);

$(window).resize(function() {
	SpamCheck.iefix();
	SpamCheck.realign();
});
