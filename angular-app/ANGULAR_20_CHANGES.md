# Key Changes from Angular 19 to 20

For .NET developers learning Angular, v20 makes things simpler:

## 1. Cleaner Template Syntax - More like C#/Razor
The new control flow syntax is much more intuitive for developers coming from .NET:

### Old Syntax (Angular 19 and earlier):
```html
<!-- Conditionals -->
<div *ngIf="isAuthenticated">Welcome!</div>

<!-- Loops -->
<li *ngFor="let user of users">{{user.name}}</li>
```

### New Syntax (Angular 20):
```html
<!-- Conditionals - feels more like C# if statements -->
@if (isAuthenticated) {
  <div>Welcome!</div>
}

<!-- Loops - similar to C# foreach -->
@for (user of users; track user.id) {
  <li>{{user.name}}</li>
}
```

## 2. Less Boilerplate - Standalone Components by Default
New components in Angular 20 are standalone by default, meaning:
- No need to declare components in NgModules
- Import only what you need directly in the component
- Simpler mental model - similar to how you work with classes in C#

## 3. Better Performance
Angular 20 includes performance improvements under the hood, but your existing code will benefit automatically without changes.

## 4. Same Core Concepts
The fundamental concepts remain unchanged:
- **Services and Dependency Injection** - Works just like .NET Core DI
- **Components** - Still your building blocks for UI
- **HTTP Client** - Same patterns for calling your WebAPI
- **JWT Authentication** - Same token-based auth flow with interceptors
- **Reactive Forms** - Same powerful form handling

## What Was Updated in This Demo

### Template Syntax Updates
All templates have been updated to use the new control flow syntax:
- `*ngIf` → `@if`
- `*ngFor` → `@for` (with required `track` expression)
- Nested conditionals now use cleaner block syntax

### HTTP Interceptors
The demo already uses the modern functional interceptor pattern:
```typescript
// Modern functional style (Angular 15+)
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // Add JWT token to requests
}
```

### No Breaking Changes
The authentication patterns, WebAPI integration, and overall architecture remain the same. You can focus on learning the concepts rather than worrying about version differences.

## For Your VSLive Presentation

Key talking points for .NET developers:
1. **Familiar Syntax**: The new `@if` and `@for` syntax will feel natural to anyone who's used Razor
2. **TypeScript = C# for the Browser**: Strong typing, classes, interfaces, generics - all familiar concepts
3. **DI Works the Same**: Angular's DI container works just like .NET Core's
4. **JWT Auth Pattern**: The same Bearer token approach you use in WebAPI

## Migration Notes
If you have existing Angular 19 code:
1. The old `*ngIf` and `*ngFor` syntax still works (but new syntax is recommended)
2. All Angular 19 code runs fine in Angular 20
3. Use `ng update` to automatically update dependencies
4. The migration is mostly about adopting new best practices, not fixing breaking changes

Focus on learning the concepts - they transfer directly between Angular versions and align well with .NET patterns you already know!