using Backend.Dao;
using Backend.Dto;
using Backend.Entity;
using Backend.ExceptionHandling;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service;

public class OrderService
{
    private readonly ApplicationDao _applicationDao;

    public OrderService(ApplicationDao applicationDao)
    {
        _applicationDao = applicationDao;
    }

    public List<OrderDto> GetUserOrders(string userId)
    {
        List<Order> userOrders = _applicationDao.Orders
                .Include(o => o.Products) // Include related products if needed
                .Where(o => o.UserId == userId)
                .ToList();

            // Map each Order entity to OrderDto
            List<OrderDto> userOrdersDto = userOrders.Select(MapOrderToOrderDto).ToList();
            return userOrdersDto;
    }

    public OrderDto GetOrder(int orderId)
    {
        Order order = _applicationDao.Orders
            .Include(o => o.Products)
            .FirstOrDefault(o => o.OrderId == orderId);

        if (order == null)
        {
            throw new BadRequestException("Order not found");
        }

        OrderDto orderDto = new OrderDto
        {
            OrderId = order.OrderId,
            TotalAmount = order.TotalAmount,
            UserId = order.UserId,
            ProductsIds = order.Products.Select(p => p.ProductId).ToList()
        };

        return orderDto;
    }

    public OrderDto CreateOrder(OrderDto orderDto)
    {
        Order order = new Order
        {
            TotalAmount = orderDto.TotalAmount,
            UserId = orderDto.UserId
        };

        // add products to order
        order.Products = _applicationDao.Products
            .Where(p => orderDto.ProductsIds.Contains(p.ProductId))
            .ToList();

        _applicationDao.Orders.Add(order);
        _applicationDao.SaveChanges();

        orderDto.OrderId = order.OrderId;
        return orderDto;
    }

    public void DeleteOrder(int orderId)
    {
        Order order = _applicationDao.Orders
            .FirstOrDefault(o => o.OrderId == orderId);

        if (order == null)
        {
            throw new BadRequestException("Order not found");
        }

        _applicationDao.Orders.Remove(order);
        _applicationDao.SaveChanges();
    }

    public OrderDto UpdateOrder(int orderId, OrderDto updatedOrderDto)
    {
        Order existingOrder = _applicationDao.Orders
            .Include(o => o.Products)
            .FirstOrDefault(o => o.OrderId == orderId);

        if (existingOrder == null)
        {
            throw new BadRequestException("Order not found");
        }

        existingOrder.TotalAmount = updatedOrderDto.TotalAmount;

        // update products as well
        existingOrder.Products.Clear();
        existingOrder.Products.AddRange(_applicationDao.Products
            .Where(p => updatedOrderDto.ProductsIds.Contains(p.ProductId))
            .ToList());

        _applicationDao.SaveChanges();

        updatedOrderDto.OrderId = existingOrder.OrderId;
        return updatedOrderDto;
    }

    private OrderDto MapOrderToOrderDto(Order order)
    {
        OrderDto orderDto = new OrderDto
        {
            OrderId = order.OrderId,
            TotalAmount = order.TotalAmount,
            UserId = order.UserId,
            ProductsIds = order.Products.Select(product => product.ProductId).ToList()
        };

        return orderDto;
    }

}