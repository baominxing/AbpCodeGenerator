using ABPCodeGenerator.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ABPCodeGenerator.Filters
{
    public class ParameterNullOrEmptyFilter : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var errorMessage = string.Empty;
            var errorCode = AppConfig.ErrorCodes.NONE;

            if (context.ActionArguments.Any())
            {
                foreach (var item in context.ActionArguments)
                {
                    if (item.Value != null)
                    {
                        if (item.Value.GetType() == typeof(string))
                        {
                            //传入参数是string字符串
                            if (string.IsNullOrEmpty(item.Value.ToString()))
                            {
                                errorMessage = $"输入参数不能为NULL";
                                errorCode = AppConfig.ErrorCodes.ERROR;

                                break;
                            }
                        }

                        //传入参数是对象类型
                        if (item.Value.GetType().IsClass && !item.Value.GetType().IsAbstract && !item.Value.GetType().IsGenericType)
                        {
                            var parameterType = item.Value.GetType();

                            foreach (var property in parameterType.GetProperties())
                            {
                                if (property.PropertyType == typeof(string))
                                {
                                    var attributes = property.GetCustomAttributes();

                                    if (attributes.Any())
                                    {
                                        foreach (var attribute in attributes)
                                        {
                                            //如果对象的string属性有RequiredAttribute,需要判断其值是否为空
                                            if (attribute.GetType() == typeof(RequiredAttribute))
                                            {
                                                var propertyValue = property.GetValue(item.Value);

                                                if (propertyValue != null)
                                                {
                                                    if (string.IsNullOrEmpty(propertyValue.ToString()))
                                                    {
                                                        errorMessage = $"输入参数{property.Name}不能为空";
                                                        errorCode = AppConfig.ErrorCodes.ERROR;

                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    errorMessage = $"输入参数{property.Name}不能为NULL";
                                                    errorCode = AppConfig.ErrorCodes.ERROR;

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        errorMessage = $"输入参数不能为NULL";
                        errorCode = AppConfig.ErrorCodes.ERROR;
                    }
                }
            }


            if (errorCode != AppConfig.ErrorCodes.NONE)
            {
                context.Result = new JsonResult(new { errorCode, errorMessage });
                context.HttpContext.Response.StatusCode = 200;
            }
        }
    }
}
