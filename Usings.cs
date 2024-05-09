// system

global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;

global using System.ComponentModel.DataAnnotations;
global using System.Data;
global using System.Diagnostics;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Cryptography;
global using System.Security.Claims;
global using System.Text;

// ours

global using Entities = bretts_services.Models.Entities;
global using bretts_services.Interfaces;
global using bretts_services.Options;
global using bretts_services.Services;
global using bretts_services.Utilities;
global using ViewModels = bretts_services.Models.ViewModels;

// third party

global using AutoMapper;
global using Serilog;
global using Serilog.Sinks.MSSqlServer;
