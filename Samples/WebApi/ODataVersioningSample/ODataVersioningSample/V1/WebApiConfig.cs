﻿using AutoMapper;
using Microsoft.Data.Edm;
using ODataVersioningSample.Extensions;
using ODataVersioningSample.Models;
using ODataVersioningSample.V1.ViewModels;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.OData.Builder;
using V2VM = ODataVersioningSample.V2.ViewModels;

namespace ODataVersioningSample.V1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Versioning by route prefix
            config.Routes.MapODataRoute("V1RouteVersioning", "versionbyroute/v1", GetModel());

            // Versioning by query string
            config.Routes.MapODataRoute("V1QueryStringVersioning", "versionbyquery", GetModel(), new { v = "1" }, null);

            // Versioning by header value
            config.Routes.MapODataRoute("V1HeaderVersioning", "versionbyheader", GetModel(), null, new { v = "1" });

            var controllerSelector = config.Services.GetService(typeof(IHttpControllerSelector)) as ODataVersionControllerSelector;
            // Mapping route name to controller versioning suffix
            controllerSelector.RouteVersionSuffixMapping.Add("V1RouteVersioning", "V1");
            controllerSelector.RouteVersionSuffixMapping.Add("V1QueryStringVersioning", "V1");
            controllerSelector.RouteVersionSuffixMapping.Add("V1HeaderVersioning", "V1");

            Mapper.CreateMap<DbProduct, Product>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => (int)src.ID))
                .ForSourceMember(src => src.Family, opt => opt.Ignore());
            Mapper.CreateMap<Product, DbProduct>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => (long)src.ID))
                .ForMember(dest => dest.Family, opt => opt.Ignore());

            Mapper.CreateMap<V2VM.Product, Product>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => (int)src.ID))
                .ForSourceMember(src => src.Family, opt => opt.Ignore());
            Mapper.CreateMap<Product, V2VM.Product>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => (long)src.ID))
                .ForMember(dest => dest.Family, opt => opt.Ignore());

            Mapper.AssertConfigurationIsValid();
        }

        private static IEdmModel GetModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Product>("Products");
            return builder.GetEdmModel();
        }
    }
}