import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject, Observable } from 'rxjs';
import { GameStateResponse } from '../models/game.model';

@Injectable({
  providedIn: 'root'
})
export class LiveStreamService {
  private hubConnection!: signalR.HubConnection;
  private stateUpdates = new Subject<GameStateResponse>();

  /**
   * Initializes the persistent SignalR socket pipeline.
   * Leverages automatic exponential backoff reconnection strategies.
   */
  public initializeSocketStream(): void {
    // Avoid double initialization if the stream is already active
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/hub/game', {
        skipNegotiation: false, // Fallback gracefully if WebSockets are blocked by proxies
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect({
        // Enterprise retry matrix (0s, 2s, 10s, 30s delays)
        nextRetryDelayInMilliseconds: retryContext => {
          if (retryContext.previousRetryCount === 0) return 0;
          if (retryContext.previousRetryCount === 1) return 2000;
          if (retryContext.previousRetryCount === 2) return 10000;
          return 30000;
        }
      })
      .configureLogging(signalR.LogLevel.Information) // Streams telemetry directly to the browser console
      .build();

    // Start connection stream
    this.hubConnection.start()
      .then(() => console.log('🚀 Successfully connected to the .NET SignalR Hub Server Cluster.'))
      .catch(err => console.error('❌ Real-time infrastructure negotiation handshake failed: ', err));

    // Register active listener for server-side push broadcasts
    this.hubConnection.on('ReceiveGameState', (state: GameStateResponse) => {
      this.stateUpdates.next(state);
    });

    // Handle lifecycle connection drop hooks for analytics/UI alerting
    this.hubConnection.onreconnecting((error) => {
      console.warn(`⚠️ Network connection pipeline disrupted: ${error}. Attempting recovery loops...`);
    });

    this.hubConnection.onreconnected((connectionId) => {
      console.log(`✅ Connection restored successfully. Re-mapped to active pipeline ID: ${connectionId}`);
    });
  }

  /**
   * Exposes the underlying message payload as a read-only RxJS stream
   * to guarantee unidirectional data flow across components.
   */
  public listenToStateStream(): Observable<GameStateResponse> {
    return this.stateUpdates.asObservable();
  }

  /**
   * Binds the current client session to a specific distributed room group on the server.
   * This isolates multiplayer tracking data strictly to individual match instances.
   */
  public bindToSessionGroup(gameId: number): void {
    // Ensure group injection requests wait for the socket state machine to turn active
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('JoinSession', gameId)
        .catch(err => console.error(`❌ Room isolation joining exception for ID [${gameId}]:`, err));
    } else {
      // Small timeout fallback if component requests group binding during immediate boot cycles
      setTimeout(() => this.bindToSessionGroup(gameId), 500);
    }
  }

  /**
   * Gracefully tears down infrastructure references when navigating out of application routes.
   */
  public destroySocketStream(gameId: number): void {
    if (this.hubConnection) {
      this.hubConnection.invoke('LeaveSession', gameId)
        .then(() => this.hubConnection.stop())
        .then(() => console.log('🛑 Real-time streaming network socket cleanly disengaged.'))
        .catch(err => console.error('Exception encountered during network resource cleanup:', err));
    }
  }
}
