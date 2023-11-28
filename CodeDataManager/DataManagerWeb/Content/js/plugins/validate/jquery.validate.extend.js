/*this is basic form validation using for validation person's basic information author:Clara Guo data:2017/07/20*/
$(document).ready(function(){
	$.validator.setDefaults({       
		  submitHandler: function(form) {    
		 		form.submit();    
		}       
	});
	//手机号码验证身份证正则合并：(^\d{15}$)|(^\d{17}([0-9]|X)$)
	jQuery.validator.addMethod("isPhone",function(value,element){
		var length = value.length;
		var phone=/^1[3|4|5|6|7|8|9][0-9]\d{8}$/;
		return this.optional(element)||(length == 11 && phone.test(value));
	},"请填写正确的11位手机号");
	//电话号码验证
	jQuery.validator.addMethod("isTel",function(value,element){
		var tel = /^(0\d{2,3}-)?\d{7,8}$/g;//区号3,4位,号码7,8位
		return this.optional(element) || (tel.test(value));
	},"请填写正确的座机号码");
	//电话号码全验证
	jQuery.validator.addMethod("isallTel",function(value,element){
		var tel = /^(0\d{2,3}-)?\d{7,8}$/g;//区号3,4位,号码7,8位
		var phone=/^1[3|4|5|6|7|8|9][0-9]\d{8}$/;
		var length = value.length;
		return  (this.optional(element) || (tel.test(value) )) || ( this.optional(element) ||  length == 11 && phone.test(value));
	},"请填写正确的座机号码");
	//姓名校验
	jQuery.validator.addMethod("isName",function(value,element){
		var name=/^[\u4e00-\u9fa5]{2,6}$/;
		return this.optional(element) || (name.test(value));
	},"姓名只能用汉字,长度2-4位");
	//校验用户名
	jQuery.validator.addMethod("isUserName",function(value,element){
		var userName=/^[a-zA-Z0-9]{2,13}$/;
		return this.optional(element) || (userName).test(value);
	},'请输入数字或者字母,不包含特殊字符');
	//校验数字
	jQuery.validator.addMethod("iszNum",function(value,element){
		var zNum=/^[0-9]{1,13}$/;
		return this.optional(element) || (zNum).test(value);
	},'请输入正整数');
	//校验浮点
	jQuery.validator.addMethod("isfNum",function(value,element){
		var fNum=/(\d*)(\.\d*)?|0/;
		return this.optional(element) || (fNum).test(value);
	},'请输入浮点数');
	//校验正浮点
	jQuery.validator.addMethod("iszfNum",function(value,element){
		var zfNum=/^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$/;
		return this.optional(element) || (zfNum).test(value);
	},'请输入正浮点数');
	//校验大于等于0浮点
	jQuery.validator.addMethod("iszzfNum",function(value,element){
		var zfNum=/^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$/;
		return this.optional(element) || (zfNum).test(value) || (value==0);
	},'请输入大于等于0');

	//校验身份证
	jQuery.validator.addMethod("isIdentity",function(value,element){
		var id= /^(\d{15}$|^\d{18}$|^\d{17}(\d|X))$/;
		return this.optional(element) || (validateIdCard(value));
	},"请输入正确的15或18位身份证号,末尾为大写X");
	//校验出生日期
	jQuery.validator.addMethod("isBirth",function(value,element){
		var birth = /^(19|20)\d{2}-(1[0-2]|0?[1-9])-(0?[1-9]|[1-2][0-9]|3[0-1])$/;
		return this.optional(element) || (birth).test(value);
	},"出生日期格式示例2000-01-01");
	//校验IP地址
	jQuery.validator.addMethod("isIp",function(value,element){
		var ip = /^(?:(?:2[0-4][0-9]\.)|(?:25[0-5]\.)|(?:1[0-9][0-9]\.)|(?:[1-9][0-9]\.)|(?:[0-9]\.)){3}(?:(?:2[0-4][0-9])|(?:25[0-5])|(?:1[0-9][0-9])|(?:[1-9][0-9])|(?:[0-9]))$/;
		return this.optional(element) || (ip).test(value);
	},"IP地址格式示例127.0.0.1");
	jQuery.validator.addMethod("notEqual", function(value, element, param) {
        return value != param;
    }, $.validator.format("输入值不允许为{0}"));
	jQuery.validator.addMethod("gt", function(value, element, param) {
        return value > param;
    }, $.validator.format("输入值必须大于{0}"));
	//校验新旧密码是否相同
	jQuery.validator.addMethod("isdiff",function(){
		var p1=$("#pwdOld").val();
		var p2=$("#pwdNew").val();
		if(p1==p2){
			return false;
		}else{
			return true;
		}
	});
	//校验日期是否后大
	jQuery.validator.addMethod("isdatel",function(){
		var p1=$("#start_date").val();
		var p2=$("#end_date").val();
		if(p1>p2){
			return false;
		}else{
			return true;
		}
	},"结束日期应大于等于开始日期");
	//校验时间是否后大
	jQuery.validator.addMethod("istimel",function(){
		var p1=$("#start_time").val();
		var p2=$("#end_time").val();
		if(p1>p2){
			return false;
		}else{
			return true;
		}
	},"结束时间应大于等于开始时间");
	//校验薪水是否后大
	jQuery.validator.addMethod("issalaryl",function(){
		var p1=parseInt($("#salary_min").val());
		var p2=parseInt($("#salary_max").val());
		if(p1>p2){
			return false;
		}else{
			return true;
		}
	},"薪水上限应大于等于薪水下限");
	//校验证件号码是否合规
	jQuery.validator.addMethod("isidcard",function(value,element){
		var p1=$("#idcard_id").val();
		var p2=$("#idcard_num").val();
		var p3=$("#member_type").val();
		var id= /^(\d{15}$|^\d{18}$|^\d{17}(\d|X))$/;
		if(p1 != '10' || p3 != '2'){
			return true;
		}else{
			return (this.optional(element) || (id.test(p2)));
		}
	},"薪水上限应大于等于薪水下限");
	//校验新密码和确认密码是否相同
	jQuery.validator.addMethod("issame",function(){
		var p3=$("#confirm_password").val();
		var p4=$("#pwdNew").val();
		if(p3==p4){
			return true;
		}else{
			 return false;
		}
		});
	//校验基础信息表单
	$("#basicInfoForm").validate({
		errorElement:'span',
		errorClass:'help-block error-mes',
		rules:{
			name:{
				required:true,
				isName:true
			},
			sex:"required",
			birth:"required",
            mobile:{
				required:true,
				isPhone:true
			},
			email:{
				required:true,
				email:true
			}
		},
		messages:{
			name:{
				required:"请输入中文姓名",
				isName:"姓名只能为汉字"
			},
			sex:{
				required:"请输入性别"
			},
			birth:{
				required:"请输入出生年月"
			},
            mobile:{
				required:"请输入手机号",
				isPhone:"请填写正确的11位手机号"
			},
			email:{
				required:"请输入邮箱",
				email:"请填写正确的邮箱格式"
			}
		},
	
		errorPlacement:function(error,element){
			element.next().remove();
			element.closest('.gg-formGroup').append(error);
		},
		
		highlight:function(element){
			$(element).closest('.gg-formGroup').addClass('has-error has-feedback');
		},
		success:function(label){
			var el = label.closest('.gg-formGroup').find("input");
			el.next().remove();
			label.closest('.gg-formGroup').removeClass('has-error').addClass("has-feedback has-success");
			label.remove();
		},
		submitHandler:function(form){
			alert("保存成功!");
		}
	});
	
	//校验修改密码表单
	$("#modifyPwd").validate({
		onfocusout: function(element) { $(element).valid()},
		 debug:false, //表示校验通过后是否直接提交表单
		 onkeyup:false, //表示按键松开时候监听验证
		rules:{
			pwdOld:{
				required:true,
				minlength:6
			},
            pwdNew:{
			   required:true,
			   minlength:6,
			   isdiff:true,
			   //issame:true,
		   },
			confirm_password:{
			  required:true,
			  minlength:6,
			  issame:true,
			}
		  
		   },
		messages:{
			 	pwdOld : {
					 required:'必填',
					 minlength:$.validator.format('密码长度要大于6')
				},
            	pwdNew:{
				   required:'必填',
				   minlength:$.validator.format('密码长度要大于6'),
				   isdiff:'原密码与新密码不能重复',
				  
			   },
				confirm_password:{
				   required:'必填',
				   minlength:$.validator.format('密码长度要大于6'),
				   issame:'新密码要与确认新密码一致',
				}
		
		},
		errorElement:"mes",
		errorClass:"gg-star",
		errorPlacement: function(error, element) 
		{ 
			element.closest('.gg-formGroup').append(error);

		}
	});
});

/*
        * 身份证15位编码规则：dddddd yymmdd xx p
        * dddddd：6位地区编码
        * yymmdd: 出生年(两位年)月日，如：910215
        * xx: 顺序编码，系统产生，无法确定
        * p: 性别，奇数为男，偶数为女
        *
        * 身份证18位编码规则：dddddd yyyymmdd xxx y
        * dddddd：6位地区编码
        * yyyymmdd: 出生年(四位年)月日，如：19910215
        * xxx：顺序编码，系统产生，无法确定，奇数为男，偶数为女
        * y: 校验码，该位数值可通过前17位计算获得
        *
        * 前17位号码加权因子为 Wi = [ 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 ]
        * 验证位 Y = [ 1, 0, 10, 9, 8, 7, 6, 5, 4, 3, 2 ]
        * 如果验证码恰好是10，为了保证身份证是十八位，那么第十八位将用X来代替
        * 校验位计算公式：Y_P = mod( ∑(Ai×Wi),11 )
        * i为身份证号码1...17 位; Y_P为校验码Y所在校验码数组位置
       */
function validateIdCard(idCard){
	//15位和18位身份证号码的正则表达式
	var regIdCard=/^(^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$)|(^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])((\d{4})|\d{3}[Xx])$)$/;

	//如果通过该验证，说明身份证格式正确，但准确性还需计算
	if(regIdCard.test(idCard)){
		if(idCard.length==18){
			var idCardWi=new Array( 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 ); //将前17位加权因子保存在数组里
			var idCardY=new Array( 1, 0, 10, 9, 8, 7, 6, 5, 4, 3, 2 ); //这是除以11后，可能产生的11位余数、验证码，也保存成数组
			var idCardWiSum=0; //用来保存前17位各自乖以加权因子后的总和
			for(var i=0;i<17;i++){
				idCardWiSum+=idCard.substring(i,i+1)*idCardWi[i];
			}
			var idCardMod=idCardWiSum%11;//计算出校验码所在数组的位置
			var idCardLast=idCard.substring(17);//得到最后一位身份证号码
			//如果等于2，则说明校验码是10，身份证号码最后一位应该是X
			if(idCardMod==2){
				if(idCardLast=="X"||idCardLast=="x"){
					return true;
					//alert("恭喜通过验证啦！");
				}else{
					return false;
					//alert("身份证号码错误！");
				}
			}else{
				//用计算出的验证码与最后一位身份证号码匹配，如果一致，说明通过，否则是无效的身份证号码
				if(idCardLast==idCardY[idCardMod]){
					return true;
					//alert("恭喜通过验证啦！");
				}else{
					return false;
					//alert("身份证号码错误！");
				}
			}
		}
	}else{
		return false;
		//alert("身份证格式不正确!");
	}
}