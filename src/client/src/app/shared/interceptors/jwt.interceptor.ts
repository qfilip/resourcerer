import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable } from "rxjs";
import { UserService } from "../../features/user/services/user.service";

@Injectable({
  providedIn: 'root'
})
export class JwtInterceptor implements HttpInterceptor {
  private service = inject(UserService);

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const jwt = this.service.$jwt();
    if (!jwt) {
      return next.handle(req);
    }
    const headers = req.headers.set('Authorization', `Bearer ${jwt}`);
    const reqClone = req.clone({ headers: headers });

    return next.handle(reqClone);
  }
}