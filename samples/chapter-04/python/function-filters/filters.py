async def function_filter(context, next):
    print("Filter applied")
    await next(context)
