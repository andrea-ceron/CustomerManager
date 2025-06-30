using AutoMapper;
using CustomerManager.Business.DTOHelper;
using CustomerManager.Repository.Model;
using CustomerManager.Shared.DTO;
using System.Diagnostics.CodeAnalysis;
using StockManager.Shared.DTO;


namespace CustomerManager.Business.Profiles;

/// <summary>
/// Marker per <see cref="AutoMapper"/>.
/// </summary>
public sealed class AssemblyMarker
{
	AssemblyMarker() { }
}

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class InputFileProfile : Profile
{
	public InputFileProfile()
	{
		CreateMap<Customer, ReadCustomerDto>();
		CreateMap<UpdateCustomerDto, Customer>();
		CreateMap<CreateCustomerDto , Customer>();
		CreateMap<Customer, ReadCustomerForSellingInvoiceControllerDto>();

		CreateMap<CreateSellingInvoiceDto, Invoice>();
		CreateMap<Invoice, ReadSellingInvoiceDto>()
			.ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
			.ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
			.ForMember(dest => dest.ProductList, opt => opt.MapFrom(src => src.ProductList));
		CreateMap<Address, ReadAndUpdateAddressDto>();
		CreateMap<ReadAndUpdateAddressDto, Address>();
		CreateMap<CreateAddressDto, Address>();

		CreateMap<InvoiceProducts, ReadInvoiceProductsDto>();
		CreateMap<ReadInvoiceProductsDto, InvoiceProducts>();
		CreateMap<CreateInvoiceProductsDto, InvoiceProducts>();

		CreateMap<CreateInvoiceProductHelper, InvoiceProducts>();


		CreateMap<CreateProductDto, Product>();
		CreateMap<Product, ReadAndUpdateProductDto>();
		CreateMap<ReadAndUpdateProductDto, Product>();
		CreateMap<EndProductDtoForKafka, Product>();

	}
}