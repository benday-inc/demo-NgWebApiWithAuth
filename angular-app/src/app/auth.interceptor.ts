import { inject, Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HttpHandlerFn } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { getBaseUrl } from '../main';


export function authInterceptorFn(
  req: HttpRequest<unknown>, 
  next: HttpHandlerFn): Observable<HttpEvent<unknown>> {
  console.log(`authInterceptorFn: ${req.url}`);

  let router = inject(Router);
  
  let url = req.url;

  if (url.startsWith('/')) {
    url = getBaseUrl() + url;
  }
  else if (!url.startsWith('http')) {
    url = getBaseUrl() + '/' + url;
  }

  const token = localStorage.getItem('token');

  if (token) {
    console.log('authInterceptorFn: Adding Authorization header');
    req = req.clone({
      url: url,
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }
  else {
    console.log('authInterceptorFn: No token found');
    req = req.clone({
      url: url
    });
  }
 
  return next(req);
}

