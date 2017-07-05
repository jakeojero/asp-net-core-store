﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using Casestudy.Utils;
using Microsoft.AspNetCore.Http;
using Casestudy.ViewModels;
using System.Text;

namespace Casestudy.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("products", Attributes = BrandNameAttribute)]
    public class ProductHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            if (_session.Get<ProductViewModel[]>("catalog") != null && BrandName != null)
            {
                var innerHtml = new StringBuilder();
                ProductViewModel[] products = _session.Get<ProductViewModel[]>("catalog");
                innerHtml.Append("<div class=\"col-xs-12\" style=\"font-size:x-large;\"><span>Catalogue</span></div>");

                foreach(ProductViewModel product in products)
                {

                    product.JsonData = JsonConvert.SerializeObject(product);
                    innerHtml.Append("<div id=\"item\" class=\"col-xs-12 col-sm-6 col-md-6 col-lg-4\"><div class=\"db-wrapper\"><div class=\"db-pricing-seven\">");
                    innerHtml.Append("<ul><li class=\"price\" style=\"padding-left:0; padding-right:0;\"><span style=\"font-size:large;\">" + product.ProductName + "<br/><img style=\"height:200px; max-width: 200px;\" src=\"/img/" + product.GraphicName + ".jpg" + "\"" + "/></span></li>");
                    innerHtml.Append("<li><span style=\"font-size:large;\">" + product.Description.Substring(0, 20) + "...</span></li>");
                    innerHtml.Append("<li><span style=\"font-size:large;\"> Our Price: <strong>$" + product.CostPrice.ToString("#.00") + "</strong></span></li>");
                    innerHtml.Append("<li><a href=\"#details_popup\" class=\"btn btn-primary\" data-toggle=\"modal\" id=\"modalbtn" + product.Id + "\" data-details='" + product.JsonData + "' data-id=\"" + product.Id + "\">Details</a></li>");
                    innerHtml.Append("</ul>");
                    innerHtml.Append("</div></div></div>");

                }
                output.Content.SetHtmlContent(innerHtml.ToString());
            }
        }

        private const string BrandNameAttribute = "brand";

        [HtmlAttributeName(BrandNameAttribute)]
        public string BrandName { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;

        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public ProductHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
}