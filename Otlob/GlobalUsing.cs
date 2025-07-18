﻿global using EFCore = Microsoft.EntityFrameworkCore.EF;
global using Otlob.Areas.Customer.Services.Interfaces;
global using Microsoft.AspNetCore.DataProtection;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.WebUtilities;
global using Otlob.Core.IUnitOfWorkRepository;
global using Microsoft.IdentityModel.Tokens;
global using Otlob.EF.UnitOfWorkRepository;
global using Otlob.Areas.Customer.Services;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.SignalR;
global using Microsoft.AspNetCore.Mvc;
global using System.Linq.Expressions;
global using System.Security.Claims;
global using Otlob.Services.Results;
global using Otlob.Core.ViewModel;
global using Otlob.Core.IServices;
global using Otlob.Core.Services;
global using System.Diagnostics;
global using Otlob.Core.Models;
global using Newtonsoft.Json;
global using Stripe.Checkout;
global using Otlob.Core.Hubs;
global using Otlob.IServices;
global using Otlob.Services;
global using Otlob.Models;
global using Otlob.EF;
global using Utility;
global using LinqKit;