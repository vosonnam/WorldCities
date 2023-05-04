import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, AsyncValidatorFn, AbstractControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '../../environments/environment';
import { City } from './city';
import { Country } from './../countries/country';
@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.scss']
})
export class CityEditComponent implements OnInit {

  title?: string;
  form!: FormGroup;
  city?: City;
  id?: number;
  countries?: Country[];

  constructor(
    private http: HttpClient,
    private router: Router,
    private activateRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.form = new FormGroup({
      name: new FormControl('', Validators.required),
      lat: new FormControl('', Validators.required),
      lon: new FormControl('', Validators.required),
      countryId: new FormControl('',Validators.required)
    },null,this.isDupeCity());
    this.loadData();
  }
  /**get city from id*/
  loadData() {
    this.loadCountries();
    var idParam = this.activateRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;
    if (this.id) {
      var url = environment.BaseUrl + 'api/Cities/' + this.id;
      this.http.get<City>(url).subscribe(
        result => {
          this.city = result;
          this.title = 'Edit - ' + this.city.name;
          this.form.patchValue(this.city);
        },
        error => console.error(error));
    } else {
      this.title = "Create a new City";
    }
  }
  /**get countries */
  loadCountries() {
    var url = environment.BaseUrl + 'api/Countries';
    this.http.get<Country[]>(url).subscribe(
      result => {
        this.countries = result;
      },
      error => console.error(error));
  }
  /**save/update city*/
  onSubmit() {
    var city = (this.id) ? this.city : <City>{};
    if (city) {
      city.name = this.form.controls['name'].value;
      city.lat = this.form.controls['lat'].value;
      city.lon = this.form.controls['lon'].value;
      city.countryId = this.form.controls['countryId'].value;
      if (this.id) {
        var url = environment.BaseUrl + 'api/Cities/' + city.id;
        this.http.put<City>(url, city).subscribe(
          _ => {
            console.log("City " + city!.id + " has been updated.");
            this.router.navigate(['/cities']);
          },
          error => console.error(error));
      }
      else {
        var url = environment.BaseUrl + 'api/Cities';
        this.http.post<City>(url, city).subscribe(
          _ => {
            console.log("City " + city!.id + " has been created.");
            this.router.navigate(['/cities']);
          },
          error => console.error(error));
      }

    }
  }
  isDupeCity(): AsyncValidatorFn {
    return (_control: AbstractControl): Observable<{ [key: string]: any } | null> => {
      var city = <City>{};
      city.id = this.id ? this.id : 0;
      city.name = this.form.controls['name'].value;
      city.lat = +this.form.controls['lat'].value;
      city.lon = +this.form.controls['lon'].value;
      city.countryId = +this.form.controls['countryId'].value;

      var url = environment.BaseUrl + 'api/Cities/IsDupeCity';
      return this.http.post<boolean>(url, city).pipe(map(result => {
        return (result ? { isDupeCity: true } : null);
      }));
    }
  }
}
