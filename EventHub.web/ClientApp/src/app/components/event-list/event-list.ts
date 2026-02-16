import { Component } from '@angular/core';




interface Event {
  title: string;
  date: string;
  location: string;
}

@Component({
  selector: 'app-event-list',
  imports: [],
  templateUrl: './event-list.html',
  styleUrl: './event-list.scss'
})
export class EventList {

}
