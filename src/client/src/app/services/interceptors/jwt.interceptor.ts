import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpResponse, HttpHeaders } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable, tap } from "rxjs";
import { SpinnerService } from "../spinner.service";
import { UserService } from "../user.service";

@Injectable({
    providedIn: 'root'
})
export class JwtInterceptor implements HttpInterceptor {
    private service = inject(UserService);

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const jwt = this.service.jwt();
        if(!jwt) {
            return next.handle(req);
        }

        const headers = new HttpHeaders().set('Bearer', jwt);
        const reqClone = req.clone({ headers: headers });

        return next.handle(reqClone);
    }
}