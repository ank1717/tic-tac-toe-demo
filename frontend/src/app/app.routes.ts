import { Routes } from '@angular/router';
import { LandingComponent } from './components/landing/landing';
import { GameBoardComponent } from './components/game-board/game-board';

export const routes: Routes = [
	{ path: '', component: LandingComponent },
	{ path: 'game', component: GameBoardComponent },
	{ path: '**', redirectTo: '' }
];
